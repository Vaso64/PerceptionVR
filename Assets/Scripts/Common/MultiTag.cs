using System.Collections.Generic;
using UnityEngine;

namespace PerceptionVR.Common
{
    public enum Tag
    {
        Teleportable,
    }

    public class MultiTag : MonoBehaviour
    {

        [SerializeField]
        public List<Tag> tags;

        public static bool ObjectHasTag(GameObject _object, Tag tag, bool checkChildren = false)
        {
            return ObjectHasTag(_object.transform, tag, checkChildren);
        }

        public static bool ObjectHasTag(Component _object, Tag tag, bool checkChildren = false)
        {
            var multiTag = _object.GetComponent<MultiTag>();
            return multiTag != null && multiTag.tags.Contains(tag);
        }
    }
}