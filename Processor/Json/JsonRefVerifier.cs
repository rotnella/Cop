using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnEGames.Cop.Processor.Json
{
    public class JsonRefVerifier
    {
        private bool _ignoreOnIdNull = false;
        private bool _ignoreOnBadIdRef = false;
        private bool _ignoreOnBadRefPath = true;

        public JsonRefVerifier IgnoreOnIdNull()
        {
            _ignoreOnIdNull = true;
            return this;
        }
        public JsonRefVerifier IgnoreOnBadIdRef()
        {
            _ignoreOnBadIdRef = true;
            return this;
        }
        public JsonRefVerifier IgnoreOnBadRefPath()
        {
            _ignoreOnBadIdRef = true;
            return this;
        }
        public JContainer VerifyRefObject(string id, IDictionary<string, JContainer> idToObjectMap)
        {
            if (!_ignoreOnBadIdRef)
            {
                VerifyArgument.ThrowIfNullOrWhitespace(nameof(id), id);
            }
            if(id == null)
            {
                return null;
            }
            if (!idToObjectMap.ContainsKey(id))
            {
                if (_ignoreOnBadIdRef)
                {
                    //add log listener
                }
                else
                {
                    throw new ArgumentException($"id:'{id}' does not match any object");
                }
            }
            return idToObjectMap[id];

        }
        public JToken VerifyRef(JToken referencedObject, string refPath)
        {
            //we have the object, but we need the field from the refPath, path is after :: in the reference
            if (refPath == null)
            {
                if (_ignoreOnBadRefPath)
                {
                    return null;
                }

                throw new ArgumentException($"refPath is null within {referencedObject.ToString()}");
            }
            if(refPath == "/")
            {
                return referencedObject;
            }
            try
            {
                return referencedObject.SelectToken("$." +refPath, errorWhenNoMatch: true);
            }
            catch (JsonException e)
            {
                if (_ignoreOnBadRefPath)
                {
                    //add log listener
                    return null;
                }
                else
                {
                    throw new ArgumentException($"refPath:'{refPath}' is not found within {referencedObject.ToString()}");
                }
            }
        }

        public string VerifyId(JToken idValue, IDictionary<string, JContainer> idToObjectMap)
        {
            string? id = null;
            if (!_ignoreOnIdNull)
            {
                if(idValue == null || string.IsNullOrWhiteSpace(idValue.ToString()))
                {
                    throw new ArgumentException($"id is null");
                }
                id = idValue.ToString();
                if (idValue.Type != JTokenType.String)
                {
                    throw new ArgumentException($"id:'{id}' is not of type 'string'");
                }

                //validate unique in doc.
                if (idToObjectMap.ContainsKey(id.ToLower()))
                {
                    throw new ArgumentException($"id:'{id}' is not unique within document");
                }
            }
            if(id == null)
            {
                //add log listener
            }
            return id;
        }
    }
}
