using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace CodySource
{
    namespace ServerCookie
    {
        public class ServerCookie : MonoBehaviour
        {

            #region PROPERTIES

            private static ServerCookie _instance;
            public static ServerCookie instance
            {
                get
                {
                    if (_instance == null)
                    {
                        ServerCookie[] _all = FindObjectsOfType<ServerCookie>();
                        _instance = _all.Length>0?_all[0]:null;
                        if (_instance != null) return _instance;
                        GameObject obj = new GameObject();
                        obj.hideFlags = HideFlags.HideAndDontSave;
                        _instance = obj.AddComponent<ServerCookie>();
                    }
                    return _instance;
                }
            }

            public string defaultURL = "";
#if UNITY_EDITOR
            public string editorOverride = "";
#endif
            public UnityEvent<string> onRequestFailed = new UnityEvent<string>();
            public UnityEvent<string> onRequestComplete = new UnityEvent<string>();

            #endregion

            #region PUBLIC METHODS

            /// <summary>
            /// Loads the manifest of the streaming assets contents
            /// </summary>
            public void RequestCookie(string pCookie = "") => StartCoroutine(_Request(defaultURL+ "?Cookie=" + pCookie));

            /// <summary>
            /// Loads the manifest of the streaming assets contents
            /// </summary>
            public void RequestCookie(string pURL = "", string pCookie = "") => StartCoroutine(_Request(((pURL == "") ? defaultURL : pURL) + "?Cookie=" + pCookie));

            #endregion

            #region INTERNAL METHODS

            /// <summary>
            /// Perform the request
            /// </summary>
            internal IEnumerator _Request(string pURL)
            {
#if UNITY_EDITOR
                if (editorOverride != "")
                {
                    onRequestComplete?.Invoke(editorOverride);
                    yield break;
                }
#endif
                WWWForm form = new WWWForm();
                using (UnityWebRequest www = UnityWebRequest.Post($"{pURL}", form))
                {
                    yield return www.SendWebRequest();
                    try
                    {
                        if (www.result != UnityWebRequest.Result.Success) onRequestFailed?.Invoke(www.error);
                        else
                        {
                            CookieResponse r = JsonConvert.DeserializeObject<CookieResponse>(www.downloadHandler.text);
                            if (r.error != "") onRequestFailed?.Invoke(r.error);
                            else onRequestComplete?.Invoke(r.value);
                        }
                    }
                    catch (System.Exception e)
                    {
                        onRequestFailed?.Invoke(e.Message);
                    }
                }
            }

            #endregion

            #region PUBLIC STRUCTS

            [System.Serializable]
            public struct CookieResponse
            {
                public string error;
                public string value;
            }

            #endregion

        }
    }
}