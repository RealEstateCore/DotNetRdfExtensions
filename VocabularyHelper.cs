// <copyright file="VocabularyHelper.cs" company="RealEstateCore Consortium">
// Copyright (c) RealEstateCore Consortium. All rights reserved.
// </copyright>

namespace RealEstateCore.DotNetRdfExtensions
{
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
#pragma warning disable SA1311 // Static readonly fields should begin with upper-case letter
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1649 // File name should match first type name
    public static class DC
    {
        public static readonly Uri title = new ("http://purl.org/dc/elements/1.1/title");
        public static readonly Uri description = new ("http://purl.org/dc/elements/1.1/description");
    }

    public static class CC
    {
        public static readonly Uri license = new ("http://creativecommons.org/ns#license");
    }

    public static class SH
    {
        public static readonly Uri NodeShape = new ("http://www.w3.org/ns/shacl#NodeShape");
        public static readonly Uri PropertyShape = new ("http://www.w3.org/ns/shacl#PropertyShape");
        public static readonly Uri property = new ("http://www.w3.org/ns/shacl#property");
        public static readonly Uri path = new ("http://www.w3.org/ns/shacl#path");
        public static readonly Uri cls = new ("http://www.w3.org/ns/shacl#class");
        public static readonly Uri targetClass = new ("http://www.w3.org/ns/shacl#targetClass");
        public static readonly Uri In = new ("http://www.w3.org/ns/shacl#in");
        public static readonly Uri datatype = new ("http://www.w3.org/ns/shacl#datatype");
        public static readonly Uri minCount = new ("http://www.w3.org/ns/shacl#minCount");
        public static readonly Uri maxCount = new ("http://www.w3.org/ns/shacl#maxCount");
        public static readonly Uri nodeKind = new ("http://www.w3.org/ns/shacl#nodeKind");
        public static readonly Uri node = new ("http://www.w3.org/ns/shacl#node");
        public static readonly Uri name = new ("http://www.w3.org/ns/shacl#name");
        public static readonly Uri description = new ("http://www.w3.org/ns/shacl#description");
        public static readonly Uri IRI = new ("http://www.w3.org/ns/shacl#IRI");
    }

    public static class RDF
    {
        public static readonly Uri type = new ("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
        public static readonly Uri first = new ("http://www.w3.org/1999/02/22-rdf-syntax-ns#first");
        public static readonly Uri rest = new ("http://www.w3.org/1999/02/22-rdf-syntax-ns#rest");
        public static readonly Uri nil = new ("http://www.w3.org/1999/02/22-rdf-syntax-ns#nil");
    }

    public static class SKOS
    {
        public static readonly Uri definition = new ("http://www.w3.org/2004/02/skos/core#definition");
    }

    public static class RDFS
    {
        public static readonly Uri label = new ("http://www.w3.org/2000/01/rdf-schema#label");
        public static readonly Uri domain = new ("http://www.w3.org/2000/01/rdf-schema#domain");
        public static readonly Uri range = new ("http://www.w3.org/2000/01/rdf-schema#range");
        public static readonly Uri comment = new ("http://www.w3.org/2000/01/rdf-schema#comment");
        public static readonly Uri subClassOf = new ("http://www.w3.org/2000/01/rdf-schema#subClassOf");
        public static readonly Uri Datatype = new ("http://www.w3.org/2000/01/rdf-schema#Datatype");
        public static readonly Uri Literal = new ("http://www.w3.org/2000/01/rdf-schema#Literal");
        public static readonly Uri Resource = new ("http://www.w3.org/2000/01/rdf-schema#Resource");
        public static readonly Uri Class = new ("http://www.w3.org/2000/01/rdf-schema#Class");
    }

    public static class OWL
    {
        public static readonly Uri Class = new ("http://www.w3.org/2002/07/owl#Class");
        public static readonly Uri ObjectProperty = new ("http://www.w3.org/2002/07/owl#ObjectProperty");
        public static readonly Uri DatatypeProperty = new ("http://www.w3.org/2002/07/owl#DatatypeProperty");
        public static readonly Uri Thing = new ("http://www.w3.org/2002/07/owl#Thing");
        public static readonly Uri Restriction = new ("http://www.w3.org/2002/07/owl#Restriction");
        public static readonly Uri FunctionalProperty = new ("http://www.w3.org/2002/07/owl#FunctionalProperty");
        public static readonly Uri versionIRI = new ("http://www.w3.org/2002/07/owl#versionIRI");
        public static readonly Uri versionInfo = new ("http://www.w3.org/2002/07/owl#versionInfo");
        public static readonly Uri deprecated = new ("http://www.w3.org/2002/07/owl#deprecated");
        public static readonly Uri oneOf = new ("http://www.w3.org/2002/07/owl#oneOf");

        public static readonly Uri annotatedSource = new ("http://www.w3.org/2002/07/owl#annotatedSource");
        public static readonly Uri annotatedProperty = new ("http://www.w3.org/2002/07/owl#annotatedProperty");
        public static readonly Uri annotatedTarget = new ("http://www.w3.org/2002/07/owl#annotatedTarget");

        public static readonly Uri onProperty = new ("http://www.w3.org/2002/07/owl#onProperty");
        public static readonly Uri onClass = new ("http://www.w3.org/2002/07/owl#onClass");
        public static readonly Uri cardinality = new ("http://www.w3.org/2002/07/owl#cardinality");
        public static readonly Uri qualifiedCardinality = new ("http://www.w3.org/2002/07/owl#qualifiedCardinality");
        public static readonly Uri allValuesFrom = new ("http://www.w3.org/2002/07/owl#allValuesFrom");
        public static readonly Uri someValuesFrom = new ("http://www.w3.org/2002/07/owl#someValuesFrom");
        public static readonly Uri minCardinality = new ("http://www.w3.org/2002/07/owl#minCardinality");
        public static readonly Uri minQualifiedCardinality = new ("http://www.w3.org/2002/07/owl#minQualifiedCardinality");
        public static readonly Uri maxCardinality = new ("http://www.w3.org/2002/07/owl#maxCardinality");
        public static readonly Uri maxQualifiedCardinality = new ("http://www.w3.org/2002/07/owl#maxQualifiedCardinality");
    }
#pragma warning restore SA1307 // Accessible fields should begin with upper-case letter
#pragma warning restore SA1311 // Static readonly fields should begin with upper-case letter
#pragma warning restore SA1402 // File may only contain a single type
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore SA1649 // File name should match first type name
}