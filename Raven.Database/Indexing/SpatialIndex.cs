//-----------------------------------------------------------------------
// <copyright file="SpatialIndex.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using Lucene.Net.Search;
using Lucene.Net.Spatial;
using Lucene.Net.Spatial.Prefix;
using Lucene.Net.Spatial.Prefix.Tree;
using Lucene.Net.Spatial.Queries;
using Raven.Abstractions.Data;
using Raven.Abstractions.Indexing;
using Spatial4n.Core.Context;
using Spatial4n.Core.Shapes;
using SpatialRelation = Spatial4n.Core.Shapes.SpatialRelation;

namespace Raven.Database.Indexing
{
	public static class SpatialIndex
	{
		// TODO: Support new SpatialContext(DistanceUnits.MILES) for backward compatibility through config
		internal static readonly SpatialContext Context = SpatialContext.GEO_KM;

		static SpatialIndex()
		{
		}

		public static SpatialStrategy CreateStrategy(string fieldName, SpatialSearchStrategy spatialSearchStrategy, int maxTreeLevel)
		{
			switch (spatialSearchStrategy)
			{
				case SpatialSearchStrategy.GeohashPrefixTree:
					return new RecursivePrefixTreeStrategy(new GeohashPrefixTree(Context, maxTreeLevel), fieldName);
				case SpatialSearchStrategy.QuadPrefixTree:
					return new RecursivePrefixTreeStrategy(new QuadPrefixTree(Context, maxTreeLevel), fieldName);
			}
			return null;
		}

		public static Query MakeQuery(SpatialStrategy spatialStrategy, string shapeWKT, SpatialRelation relation, double distanceErrorPct = 0.025)
		{
			var args = new SpatialArgs(SpatialOperation.IsWithin, Context.ReadShape(shapeWKT));
			args.SetDistPrecision(distanceErrorPct);
			return spatialStrategy.MakeQuery(args);
		}

		public static Filter MakeFilter(SpatialStrategy spatialStrategy, IndexQuery indexQuery)
		{
			var spatialQry = indexQuery as SpatialIndexQuery;
			if (spatialQry == null) return null;

			var args = new SpatialArgs(SpatialOperation.IsWithin, Context.ReadShape(spatialQry.QueryShape));
			return spatialStrategy.MakeFilter(args);
		}

		public static double GetDistance(double fromLat, double fromLng, double toLat, double toLng)
		{
			var ptFrom = Context.MakePoint(fromLng, fromLat);
			var ptTo = Context.MakePoint(toLng, toLat);
			return Context.GetDistCalc().Distance(ptFrom, ptTo);
		}
	}
}