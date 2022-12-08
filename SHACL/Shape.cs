// <copyright file="Shape.cs" company="RealEstateCore Consortium">
// Copyright (c) RealEstateCore Consortium. All rights reserved.
// </copyright>

namespace RealEstateCore.DotNetRdfExtensions.SHACL
{
    using VDS.RDF;
    using VDS.RDF.Nodes;
    using VDS.RDF.Shacl;
    using VDS.RDF.Writing.Formatting;

    public class Shape : IEquatable<Shape>
    {
        protected internal Shape(INode node, ShapesGraph graph)
        {
            this.Node = node;
            this.Graph = graph;
            this.Formatter = new TurtleFormatter(this.Graph);
        }

        public IUriNode? NodeKind
        {
            get
            {
                IUriNode shNodeKind = this.Graph.CreateUriNode(SH.nodeKind);
                return this.Graph.GetTriplesWithSubjectPredicate(this.Node, shNodeKind).Select(trip => trip.Object).UriNodes().FirstOrDefault();
            }

            set
            {
                IUriNode shNodeKind = this.Graph.CreateUriNode(SH.nodeKind);
                IUriNode? currentNodeKind = this.Graph.GetTriplesWithSubjectPredicate(this.Node, shNodeKind).Select(trip => trip.Object).UriNodes().FirstOrDefault();
                if (currentNodeKind != null)
                {
                    this.Graph.Retract(this.Node, shNodeKind, currentNodeKind);
                }

                this.Graph.Assert(this.Node, shNodeKind, value);
            }
        }

        public INode Node { get; }

        public ShapesGraph Graph { get; }

        public INodeFormatter Formatter { get; }

        public bool IsDeprecated
        {
            get
            {
                IUriNode owlDeprecated = this.Graph.CreateUriNode(OWL.deprecated);
                return this.Graph.GetTriplesWithSubjectPredicate(this.Node, owlDeprecated).Objects().LiteralNodes().Any(deprecationNode => deprecationNode.AsValuedNode().AsBoolean() == true);
            }
        }

        // Implementation of IEquatable<T> interface
        public bool Equals(Shape? shape)
        {
            return this.Node == shape?.Node;
        }

        public void AddLabel(string language, string value)
        {
            IUriNode rdfsLabel = this.Graph.CreateUriNode(RDFS.label);
            ILiteralNode labelNode = this.Graph.CreateLiteralNode(value, language);
            this.Graph.Assert(this.Node, rdfsLabel, labelNode);
        }

        public void AddComment(string language, string value)
        {
            IUriNode rdfsComment = this.Graph.CreateUriNode(RDFS.comment);
            ILiteralNode commentNode = this.Graph.CreateLiteralNode(value, language);
            this.Graph.Assert(this.Node, rdfsComment, commentNode);
        }
    }
}