// <copyright file="Shape.cs" company="RealEstateCore Consortium">
// Copyright (c) RealEstateCore Consortium. All rights reserved.
// </copyright>

namespace RealEstateCore.DotNetRdfExtensions.SHACL
{
    using VDS.RDF;
    using VDS.RDF.Nodes;
    using VDS.RDF.Shacl;
    using VDS.RDF.Writing.Formatting;

    /// <summary>
    /// Base class with shared functionality for <see cref="NodeShape"/> and <see cref="PropertyShape"/>.
    /// </summary>
    public class Shape : IEquatable<Shape>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Shape"/> class.
        /// </summary>
        /// <param name="node">RDF graph node that holds this shape's identity.</param>
        protected internal Shape(INode node)
        {
            this.Node = node;
        }

        /// <summary>
        /// Gets the RDF graph node that holds this shape's identity.
        /// </summary>
        public INode Node { get; }

        /// <summary>
        /// Gets the ShapesGraph in which this Shape is defined.
        /// </summary>
        public ShapesGraph Graph => (ShapesGraph)this.Node.Graph;

        /// <summary>
        /// Gets a value indicating whether this shape is owl:deprecated.
        /// </summary>
        public bool IsDeprecated => this.Node.IsOwlDeprecated();

        /// <summary>
        /// Gets or sets a sh:nodeKind assertion on this Shape.
        /// </summary>
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

        /// <summary>
        /// Reference equality comparison.
        /// </summary>
        /// <param name="shape">Shape to compare against.</param>
        /// <returns><c>true</c> if this is the same object as the input, else <c>false</c>.</returns>
        public bool Equals(Shape? shape)
        {
            return this.Node == shape?.Node;
        }

        /// <summary>
        /// Adds an rdfs:label annotation to this Shape.
        /// </summary>
        /// <param name="language">Label language tag.</param>
        /// <param name="value">Label text.</param>
        public void AddLabel(string language, string value)
        {
            IUriNode rdfsLabel = this.Graph.CreateUriNode(RDFS.label);
            ILiteralNode labelNode = this.Graph.CreateLiteralNode(value, language);
            this.Graph.Assert(this.Node, rdfsLabel, labelNode);
        }

        /// <summary>
        /// Adds an rdfs:comment annotation to this Shape.
        /// </summary>
        /// <param name="language">Comment language tag.</param>
        /// <param name="value">Comment text.</param>
        public void AddComment(string language, string value)
        {
            IUriNode rdfsComment = this.Graph.CreateUriNode(RDFS.comment);
            ILiteralNode commentNode = this.Graph.CreateLiteralNode(value, language);
            this.Graph.Assert(this.Node, rdfsComment, commentNode);
        }
    }
}