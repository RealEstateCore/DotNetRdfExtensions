// <copyright file="NodeShape.cs" company="RealEstateCore Consortium">
// Copyright (c) RealEstateCore Consortium. All rights reserved.
// </copyright>

namespace RealEstateCore.DotNetRdfExtensions.SHACL
{
    using VDS.RDF;
    using VDS.RDF.Query.Builder;

    /// <summary>
    /// A SHACL NodeSHape.
    /// </summary>
    public class NodeShape : Shape
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeShape"/> class.
        /// </summary>
        /// <param name="node">URI node that carries this NodeShape's identity.</param>
        public NodeShape(IUriNode node)
            : base(node)
        {
        }

        /// <summary>
        /// Gets the RDF graph node that holds this shape's identity.
        /// </summary>
        public new IUriNode Node => (IUriNode)base.Node;

        /// <summary>
        /// Gets the URI of this shape (shortcut for getting Uri property from <see cref="Node"/>).
        /// </summary>
        public Uri Uri => this.Node.Uri;

        /// <summary>
        /// Gets all SHACL PropertyShapes defined on this NodeShape.
        /// </summary>
        public IEnumerable<PropertyShape> PropertyShapes
        {
            get
            {
                IUriNode shProperty = this.Graph.CreateUriNode(SH.property);
                foreach (Triple t in this.Graph.GetTriplesWithSubjectPredicate(this.Node, shProperty))
                {
                    yield return new PropertyShape(t.Object);
                }
            }
        }

        /// <summary>
        /// Gets all direct supershapes (i.e., via <c>rdfs:subClassOf</c>) of this shape.
        /// </summary>
        public IEnumerable<NodeShape> DirectSuperShapes
        {
            get
            {
                foreach (IUriNode superClass in this.Node.DirectSuperClasses().Where(superClass => superClass.IsNodeShape()))
                {
                    yield return new NodeShape(superClass);
                }
            }
        }

        /// <summary>
        /// Gets all supershapes (i.e., via <c>rdfs:subClassOf</c>, transitively) of this shape.
        /// </summary>
        public IEnumerable<NodeShape> TransitiveSuperShapes
        {
            get
            {
                foreach (IUriNode superClass in this.Node.TransitiveSuperClasses().Where(superClass => superClass.IsNodeShape()))
                {
                    yield return new NodeShape(superClass);
                }
            }
        }

        /// <summary>
        /// Gets all direct subshapes (i.e., via <c>rdfs:subClassOf</c>) of this shape.
        /// </summary>
        public IEnumerable<NodeShape> DirectSubShapes
        {
            get
            {
                foreach (IUriNode subClass in this.Node.DirectSubClasses().Where(subClass => subClass.IsNodeShape()))
                {
                    yield return new NodeShape(subClass);
                }
            }
        }

        /// <summary>
        /// Gets all subshapes (i.e., via <c>rdfs:subClassOf</c>, transitively) of this shape.
        /// </summary>
        public IEnumerable<NodeShape> TransitiveSubShapes
        {
            get
            {
                foreach (IUriNode subClass in this.Node.TransitiveSubClasses().Where(subClass => subClass.IsNodeShape()))
                {
                    yield return new NodeShape(subClass);
                }
            }
        }

        /// <summary>
        /// Gets the longest inheritance path from this shape to a root shape, following <c>rdfs:subClassOf</c> links.
        /// </summary>
        public List<IUriNode> LongestSuperShapesPath
        {
            get
            {
                IEnumerable<NodeShape> directSuperShapes = this.DirectSuperShapes.Where(superShape => !superShape.IsDeprecated);
                if (directSuperShapes.Count() < 1 || directSuperShapes.Any(superClass => superClass.IsTopThing))
                {
                    return new List<IUriNode>();
                }
                else
                {
                    // Assume the first parent has the longest path; if not, it will be replaced in subsequent foreach
                    NodeShape longestParent = directSuperShapes.First();
                    List<IUriNode> longestParentPath = longestParent.LongestSuperShapesPath;

                    // Iterate through the other parents to see if any is longer
                    foreach (NodeShape possibleSuperShape in directSuperShapes.Skip(1))
                    {
                        List<IUriNode> possibleSuperShapeParents = possibleSuperShape.LongestSuperShapesPath;
                        if (possibleSuperShapeParents.Count() > longestParentPath.Count())
                        {
                            longestParent = possibleSuperShape;
                            longestParentPath = possibleSuperShapeParents;
                        }
                    }

                    // At this point longestParentPath + longestParent should together contain the longest path to the root; return them
                    longestParentPath.Add(longestParent.Node);
                    return longestParentPath;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this shape is a top-level thing, i.e., owl:Thing or rdfs:Resource.
        /// </summary>
        public bool IsTopThing
        {
            get
            {
                string[] topLevelThings =
                {
                    OWL.Thing.AbsoluteUri,
                    RDFS.Resource.AbsoluteUri,
                };
                return topLevelThings.Contains(this.Node.Uri.AbsoluteUri);
            }
        }

        /// <summary>
        /// Adds an assertion that this shape is an <c>rdfs:subClassOf</c> another NodeShape.
        /// </summary>
        /// <param name="superShape">The added superclass NodeShape.</param>
        public void AddSuperClass(NodeShape superShape)
        {
            this.AddSuperClass(superShape.Node);
        }

        /// <summary>
        /// Adds an assertion that this shape is an <c>rdfs:subClassOf</c> another URI node.
        /// </summary>
        /// <param name="superClass">The added superclass URI node.</param>
        public void AddSuperClass(IUriNode superClass)
        {
            IUriNode rdfsSubClassOf = this.Graph.CreateUriNode(RDFS.subClassOf);
            this.Graph.Assert(this.Node, rdfsSubClassOf, superClass);
        }

        /// <summary>
        /// Creates a new PropertyShape on this NodeShape, with the given input path. Note that only single-predicate paths (i.e., URIs) are supported.
        /// </summary>
        /// <param name="path">The sh:Path of the new property shape.</param>
        /// <returns>The newly created PropertyShape.</returns>
        public PropertyShape CreatePropertyShape(Uri path)
        {
            IBlankNode pShapeNode = this.Graph.CreateBlankNode();
            IUriNode rdfType = this.Graph.CreateUriNode(RDF.type);
            IUriNode shPropertyShape = this.Graph.CreateUriNode(SH.PropertyShape);
            this.Graph.Assert(pShapeNode, rdfType, shPropertyShape);

            IUriNode shPath = this.Graph.CreateUriNode(SH.path);
            IUriNode pathNode = this.Graph.CreateUriNode(path);
            this.Graph.Assert(pShapeNode, shPath, pathNode);

            IUriNode shProperty = this.Graph.CreateUriNode(SH.property);
            this.Graph.Assert(this.Node, shProperty, pShapeNode);

            return new PropertyShape(pShapeNode);
        }
    }
}