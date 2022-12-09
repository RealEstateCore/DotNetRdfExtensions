// <copyright file="PropertyShape.cs" company="RealEstateCore Consortium">
// Copyright (c) RealEstateCore Consortium. All rights reserved.
// </copyright>

namespace RealEstateCore.DotNetRdfExtensions.SHACL
{
    using VDS.RDF;
    using VDS.RDF.Parsing;

    /// <summary>
    /// A SHACL PropertyShape.
    /// </summary>
    public class PropertyShape : Shape
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyShape"/> class.
        /// </summary>
        /// <param name="node">URI node that carries this PropertyShape's identity.</param>
        public PropertyShape(INode node)
            : base(node)
        {
        }

        /// <summary>
        /// Gets the sh:Path of this PropertyShape.
        /// </summary>
        public INode Path
        {
            get
            {
                IUriNode shPath = this.Graph.CreateUriNode(SH.path);
                return this.Graph.GetTriplesWithSubjectPredicate(this.Node, shPath).First().Object;
            }
        }

        /// <summary>
        /// Gets the asserted sh:datatype of this PropertyShape if defined, else null.
        /// </summary>
        public IUriNode? Datatype
        {
            get
            {
                IUriNode shDatatype = this.Graph.CreateUriNode(SH.datatype);
                return this.Graph.GetTriplesWithSubjectPredicate(this.Node, shDatatype).Select(trip => trip.Object).UriNodes().FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the asserted sh:class of this PropertyShape if defined, else null.
        /// </summary>
        public IEnumerable<IUriNode> Class
        {
            get
            {
                IUriNode shClass = this.Graph.CreateUriNode(SH.cls);
                return this.Graph.GetTriplesWithSubjectPredicate(this.Node, shClass).Select(trip => trip.Object).UriNodes();
            }
        }

        /// <summary>
        /// Gets an enumerator over any asserted sh:in entries of this PropertyShape.
        /// </summary>
        public IEnumerable<INode> In
        {
            get
            {
                IUriNode shIn = this.Graph.CreateUriNode(SH.In);
                IEnumerable<INode> inListRoots = this.Graph.GetTriplesWithSubjectPredicate(this.Node, shIn).Objects();
                return inListRoots.SelectMany(listItem => this.Graph.GetListItems(listItem));
            }
        }

        /// <summary>
        /// Gets an enumerator over any asserted sh:name entries of this PropertyShape.
        /// </summary>
        public IEnumerable<ILiteralNode> Names
        {
            get
            {
                IUriNode shName = this.Graph.CreateUriNode(SH.name);
                return this.Graph.GetTriplesWithSubjectPredicate(this.Node, shName).Objects().LiteralNodes();
            }
        }

        /// <summary>
        /// Gets an enumerator over any asserted sh:description entries of this PropertyShape.
        /// </summary>
        public IEnumerable<ILiteralNode> Descriptions
        {
            get
            {
                IUriNode shDescription = this.Graph.CreateUriNode(SH.description);
                return this.Graph.GetTriplesWithSubjectPredicate(this.Node, shDescription).Objects().LiteralNodes();
            }
        }

        /// <summary>
        /// Gets or sets the asserted sh:minCount value of this PropertyShape.
        /// If no sh:minCount if defined, will get null. Will throw if sh:minCount is not an integer,
        /// or if it is defined multiple times.
        /// Setting will replace any existing sh:minCount assertions.
        /// </summary>
        public int? MinCount
        {
            get
            {
                IUriNode shMinCount = this.Graph.CreateUriNode(SH.minCount);
                IEnumerable<int> minCounts = this.Graph.GetTriplesWithSubjectPredicate(this.Node, shMinCount)
                    .Objects()
                    .LiteralNodes()
                    .Select(node => int.Parse(node.Value));
                if (minCounts.Count() == 1)
                {
                    return minCounts.First();
                }
                else
                {
                    return null;
                }
            }

            set
            {
                IUriNode shMinCount = this.Graph.CreateUriNode(SH.minCount);
                var currentMinCountTriples = this.Graph.GetTriplesWithSubjectPredicate(this.Node, shMinCount).ToList();
                this.Graph.Retract(currentMinCountTriples);
                if (value != null)
                {
                    ILiteralNode minCountNode = this.Graph.CreateLiteralNode(value.ToString(), UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeInteger));
                    this.Graph.Assert(this.Node, shMinCount, minCountNode);
                }
            }
        }

        /// <summary>
        /// Gets or sets the asserted sh:maxCount value of this PropertyShape.
        /// If no sh:maxCount if defined, will get null. Will throw if sh:maxCount is not an integer,
        /// or if it is defined multiple times.
        /// Setting will replace any existing sh:maxCount assertions.
        /// </summary>
        public int? MaxCount
        {
            get
            {
                IUriNode shMaxCount = this.Graph.CreateUriNode(SH.maxCount);
                IEnumerable<int> maxCounts = this.Graph.GetTriplesWithSubjectPredicate(this.Node, shMaxCount)
                    .Objects()
                    .LiteralNodes()
                    .Select(node => int.Parse(node.Value));
                if (maxCounts.Count() == 1)
                {
                    return maxCounts.First();
                }
                else
                {
                    return null;
                }
            }

            set
            {
                IUriNode shMaxCount = this.Graph.CreateUriNode(SH.maxCount);
                var currentMaxCountTriples = this.Graph.GetTriplesWithSubjectPredicate(this.Node, shMaxCount).ToList();
                this.Graph.Retract(currentMaxCountTriples);
                if (value != null)
                {
                    ILiteralNode maxCountNode = this.Graph.CreateLiteralNode(value.ToString(), UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeInteger));
                    this.Graph.Assert(this.Node, shMaxCount, maxCountNode);
                }
            }
        }

        /// <summary>
        /// Adds a node to the sh:in RDF list asserted on this PropertyShape, creating that list and
        /// assertion if it does not already exist.
        /// </summary>
        /// <param name="inNode">Node to be added to sh:in list.</param>
        public void AddIn(INode inNode)
        {
            IUriNode shIn = this.Graph.CreateUriNode(SH.In);
            List<INode> newInList = new () { inNode };
            INode? existingInListRoot = this.Graph.GetTriplesWithSubjectPredicate(this.Node, shIn).Objects().FirstOrDefault();
            if (existingInListRoot != null)
            {
                this.Graph.AddToList(existingInListRoot, newInList);
            }
            else
            {
                INode newInListRoot = this.Graph.AssertList(newInList);
                this.Graph.Assert(this.Node, shIn, newInListRoot);
            }
        }

        /// <summary>
        /// Adds an sh:datatype assertion on this PropertyShape. Will NOT replace any existing sh:datatype assertions.
        /// </summary>
        /// <param name="dt">Target of new sh:datatype assertion.</param>
        public void AddDatatype(Uri dt)
        {
            IUriNode shDatatype = this.Graph.CreateUriNode(SH.datatype);
            IUriNode dtNode = this.Graph.CreateUriNode(dt);
            this.Graph.Assert(this.Node, shDatatype, dtNode);
        }

        /// <summary>
        /// Adds an sh:class assertion on this PropertyShape. Will NOT replace any existing sh:class assertions.
        /// </summary>
        /// <param name="cls">Target of new sh:class assertion.</param>
        public void AddClass(Uri cls)
        {
            IUriNode shClass = this.Graph.CreateUriNode(SH.cls);
            IUriNode clsNode = this.Graph.CreateUriNode(cls);
            this.Graph.Assert(this.Node, shClass, clsNode);
        }

        /// <summary>
        /// Adds an sh:name assertion on this PropertyShape. Will NOT replace any existing sh:name assertions.
        /// </summary>
        /// <param name="language">Name language tag.</param>
        /// <param name="value">Name value.</param>
        public void AddName(string language, string value)
        {
            IUriNode shName = this.Graph.CreateUriNode(SH.name);
            ILiteralNode nameNode = this.Graph.CreateLiteralNode(value, language);
            this.Graph.Assert(this.Node, shName, nameNode);
        }

        /// <summary>
        /// Adds an sh:node assertion on this PropertyShape. Will NOT replace any existing sh:node assertions.
        /// </summary>
        /// <param name="node">Target of new sh:node assertion.</param>
        public void AddNode(Uri node)
        {
            IUriNode nodeTarget = this.Graph.CreateUriNode(node);
            IUriNode shNode = this.Graph.CreateUriNode(SH.node);
            this.Graph.Assert(this.Node, shNode, nodeTarget);
        }

        /// <summary>
        /// Adds an sh:description assertion on this PropertyShape. Will NOT replace any existing sh:description assertions.
        /// </summary>
        /// <param name="language">Description language tag.</param>
        /// <param name="value">Description value.</param>
        public void AddDescription(string language, string value)
        {
            IUriNode shDescription = this.Graph.CreateUriNode(SH.description);
            ILiteralNode descriptionNode = this.Graph.CreateLiteralNode(value, language);
            this.Graph.Assert(this.Node, shDescription, descriptionNode);
        }
    }
}