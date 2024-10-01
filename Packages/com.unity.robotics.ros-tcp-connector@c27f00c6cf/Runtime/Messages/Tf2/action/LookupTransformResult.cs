//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Tf2
{
    [Serializable]
    public class LookupTransformResult : Message
    {
        public const string k_RosMessageName = "tf2_msgs/LookupTransform";
        public override string RosMessageName => k_RosMessageName;

        public Geometry.TransformStampedMsg transform;
        public TF2ErrorMsg error;

        public LookupTransformResult()
        {
            this.transform = new Geometry.TransformStampedMsg();
            this.error = new TF2ErrorMsg();
        }

        public LookupTransformResult(Geometry.TransformStampedMsg transform, TF2ErrorMsg error)
        {
            this.transform = transform;
            this.error = error;
        }

        public static LookupTransformResult Deserialize(MessageDeserializer deserializer) => new LookupTransformResult(deserializer);

        private LookupTransformResult(MessageDeserializer deserializer)
        {
            this.transform = Geometry.TransformStampedMsg.Deserialize(deserializer);
            this.error = TF2ErrorMsg.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.transform);
            serializer.Write(this.error);
        }

        public override string ToString()
        {
            return "LookupTransformResult: " +
            "\ntransform: " + transform.ToString() +
            "\nerror: " + error.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize, MessageSubtopic.Result);
        }
    }
}