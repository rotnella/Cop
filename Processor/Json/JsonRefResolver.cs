using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace BnEGames.Cop.Processor.Json
{
    /// <summary>
    /// determine if it has references to resolve
    ///object A
    ///object B
    ///  ref A
    ///object C
    ///  ref A, ref B
    ///  
    ///lookup table 
    ///refA = [object B, object C]
    ///refB = [object C]
    ///
    ///object A completes, resolve object B & C
    /// </summary>
    public class JsonRefResolver
    {
        private readonly JsonRefMatcher _refMatcher;
        private readonly JsonRefVerifier _verifier;
        private IDictionary<string, JContainer> idToObjectMap = new Dictionary<string, JContainer>();
        private IDictionary<string, List<JsonRef>> idToObjectReference = new Dictionary<string, List<JsonRef>>();
        private Stack<JContainer> readyObjects = new Stack<JContainer>();

        public JsonRefResolver(JsonRefMatcher refMatcher = null, JsonRefVerifier verifier = null)
        {
            _refMatcher = refMatcher == null ? new JsonRefMatcher() : refMatcher;
            _verifier = verifier == null ? new JsonRefVerifier() : verifier;
        }

        public void Resolve(JContainer container)
        {
            IEnumerable<JToken> operationObjects = container.SelectTokens("$.operations[*]");
            foreach (JContainer operationObject in operationObjects)
            {
                string id = _verifier.VerifyId(operationObject[JsonRefMatcher.IdPropertyName], idToObjectMap);
                if (id != null)
                {
                    idToObjectMap[id] = operationObject;

                    IEnumerable<JValue> valuesInObjectWithRef = GetTokensWithReferences(operationObject);
                    if (valuesInObjectWithRef.Count() == 0)
                    {
                        //there are no object references, so add it to the ready stack
                        readyObjects.Push(operationObject);
                    }
                    else
                    {
                        //found object references that need resolving
                        foreach (JValue valueInObjectWithRef in valuesInObjectWithRef)
                        {
                            List<JsonRef> refs = _refMatcher.GetRefsForValue(id, valueInObjectWithRef);
                            foreach (JsonRef jsonRef in refs)
                            {
                                if (!idToObjectReference.ContainsKey(jsonRef.RefObjectId))
                                {
                                    idToObjectReference[jsonRef.RefObjectId] = new List<JsonRef>();
                                }
                                idToObjectReference[jsonRef.RefObjectId].Add(jsonRef);
                            }
                        }
                    }
                }
            }
            //resolve all the references we can
            ResolveReferences();
        }

        public JContainer TakeReadyObject()
        {
            return readyObjects.Pop();
        }
        public bool HasReadyObject()
        {
            return readyObjects.Count > 0;
        }

        private void ResolveReferences()
        {
            foreach (string id in idToObjectReference.Keys)
            {
                ResolveFor(id);
            }
        }
        public void ResolveFor(string id)
        {
            if (id == null) return;
            if (idToObjectReference.ContainsKey(id))
            {
                List<JsonRef> objectsReferencingThisObject = idToObjectReference[id];
                List<JsonRef> resolvedObjects = new List<JsonRef>();
                //resolve reference
                foreach (JsonRef referringObjectRef in objectsReferencingThisObject)
                {
                    if(ResolveReference(referringObjectRef))
                    {
                        resolvedObjects.Add(referringObjectRef);
                    }
                }
                resolvedObjects.ForEach(r => objectsReferencingThisObject.Remove(r));
                if(objectsReferencingThisObject.Count == 0)
                {
                    idToObjectReference.Remove(id);
                }
            }
        }

        /// <summary>
        /// Resolves a field value within a Json document of the form "key" : "@<xxx::operations[0].title>"
        /// where xxx is the id of the object to which it's referring, "operation" : [{ "id" : "xxx", "title" : "abc" } ]
        /// and everything after :: is the path to the field value being referenced.
        /// This method takes the reference variable, looks up the refeering object/field and replaces the variable
        /// with it's value. 
        /// Which will resolve to "key" : "abc" 
        /// </summary>
        private bool ResolveReference(JsonRef referringObjectRef)
        {
            JContainer sourceObject = idToObjectMap[referringObjectRef.SourceId];
            JContainer refObject = _verifier.VerifyRefObject(referringObjectRef.RefObjectId, idToObjectMap);
            if (refObject == null)
                return false;

            JToken referencedValueAtPath = _verifier.VerifyRef(refObject, referringObjectRef.RefPath);
            if (referencedValueAtPath == null)
                return false;

            JToken sourceValueWithRef = _verifier.VerifyRef(sourceObject.Root, referringObjectRef.SourceValuePath);
            string sourceValueWithRefAsString = sourceValueWithRef.ToString();
            if (sourceValueWithRefAsString.Length == referringObjectRef.RefVariable.Length)
            {
                //single reference only, replace object with referenced object
                sourceValueWithRef.Replace(referencedValueAtPath);
            }
            else
            {
                //there is other text or another reference in our string  val@<ref::x>is@<ref::y>
                //we are only resolving our current match
                int indexOfCurrentRef = sourceValueWithRefAsString.IndexOf(referringObjectRef.RefVariable);
                int refVariableStringLength = referringObjectRef.RefVariable.Length;
                string valuePrefix = sourceValueWithRefAsString.Substring(0, indexOfCurrentRef);
                string valueSuffix = sourceValueWithRefAsString.Substring(indexOfCurrentRef + refVariableStringLength);
                //replace the reference maintaining any prefix and suffix strings
                sourceValueWithRef.Replace(new JValue(valuePrefix + referencedValueAtPath?.ToString() + valueSuffix));
            }
            

            //check to see if object is now ready
            if (!_refMatcher.IsMatch(sourceObject.ToString()))
            {
                readyObjects.Push(sourceObject);
            }
            return true;
        }

        private IEnumerable<JValue> GetTokensWithReferences(JContainer container)
        {
            return container
              .Descendants()
              .OfType<JValue>()
              .Where(o => _refMatcher.IsMatch(o.ToString()));
        }
    }
}

