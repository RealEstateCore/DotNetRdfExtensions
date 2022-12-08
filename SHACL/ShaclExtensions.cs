// <copyright file="ShaclExtensions.cs" company="RealEstateCore Consortium">
// Copyright (c) RealEstateCore Consortium. All rights reserved.
// </copyright>

namespace RealEstateCore.DotNetRdfExtensions.SHACL
{
    using VDS.RDF;
    using VDS.RDF.Shacl;

    /// <summary>
    /// SHACL-specific extension methods.
    /// </summary>
    public static class ShaclExtensions
    {
        /// <summary>
        /// Returns all SHACL NodeShapes in this shapes graph.
        /// </summary>
        /// <param name="graph">Input shapes graph.</param>
        /// <returns>All nodes in the graph that are asserted to be <c>rdf:type</c> <c>sh:NodeShape</c>.</returns>
        public static IEnumerable<NodeShape> NodeShapes(this ShapesGraph graph)
        {
            IUriNode rdfType = graph.CreateUriNode(RDF.type);
            IUriNode shNodeShape = graph.CreateUriNode(SH.NodeShape);
            foreach (Triple t in graph.GetTriplesWithPredicateObject(rdfType, shNodeShape))
            {
                if (t.Subject.NodeType == NodeType.Uri)
                {
                    yield return new NodeShape((IUriNode)t.Subject);
                }
            }
        }

        /// <summary>
        /// Creates a new SHACL NodeShape in this shapes graph.
        /// </summary>
        /// <param name="graph">Input shapes graph.</param>
        /// <param name="uri">URI of new NodeShape.</param>
        /// <returns>The newly created NodeShape.</returns>
        public static NodeShape CreateNodeShape(this ShapesGraph graph, Uri uri)
        {
            IUriNode node = graph.CreateUriNode(uri);
            IUriNode rdfType = graph.CreateUriNode(RDF.type);
            IUriNode shNodeShape = graph.CreateUriNode(SH.NodeShape);
            graph.Assert(node, rdfType, shNodeShape);
            return new NodeShape(node);
        }

        /// <summary>
        /// Checks whether this node is a SHACL NodeShape.
        /// </summary>
        /// <param name="node">Input node.</param>
        /// <returns><c>true</c> if the input node is asserted to be <c>rdf:type</c> <c>sh:NodeShape</c>.</returns>
        public static bool IsNodeShape(this INode node)
        {
            IGraph graph = node.Graph;
            IUriNode rdfType = graph.CreateUriNode(RDF.type);
            IUriNode shNodeShape = graph.CreateUriNode(SH.NodeShape);
            return graph.ContainsTriple(node, rdfType, shNodeShape);
        }
    }
}