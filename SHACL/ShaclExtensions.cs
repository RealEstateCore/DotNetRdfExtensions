// <copyright file="ShaclExtensions.cs" company="RealEstateCore Consortium">
// Copyright (c) RealEstateCore Consortium. All rights reserved.
// </copyright>

namespace RealEstateCore.DotNetRdfExtensions.SHACL
{
    using VDS.RDF;
    using VDS.RDF.Shacl;

    public static class ShaclExtensions
    {
        public static IEnumerable<NodeShape> NodeShapes(this ShapesGraph graph)
        {
            IUriNode rdfType = graph.CreateUriNode(RDF.type);
            IUriNode shNodeShape = graph.CreateUriNode(SH.NodeShape);
            foreach (Triple t in graph.GetTriplesWithPredicateObject(rdfType, shNodeShape))
            {
                if (t.Subject.NodeType == NodeType.Uri)
                {
                    yield return new NodeShape((IUriNode)t.Subject, graph);
                }
            }
        }

        public static NodeShape CreateNodeShape(this ShapesGraph graph, Uri uri)
        {
            IUriNode node = graph.CreateUriNode(uri);
            IUriNode rdfType = graph.CreateUriNode(RDF.type);
            IUriNode shNodeShape = graph.CreateUriNode(SH.NodeShape);
            graph.Assert(node, rdfType, shNodeShape);
            return new NodeShape(node, graph);
        }

        public static bool IsNodeShape(this INode node)
        {
            IGraph graph = node.Graph;
            IUriNode rdfType = graph.CreateUriNode(RDF.type);
            IUriNode shNodeShape = graph.CreateUriNode(SH.NodeShape);
            return graph.ContainsTriple(node, rdfType, shNodeShape);
        }
    }
}