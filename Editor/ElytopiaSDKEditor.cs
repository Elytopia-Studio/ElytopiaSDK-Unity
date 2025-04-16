using Elytopia;
using UnityEditor;
using UnityEngine;

namespace ElytopiaEditor.Editor
{
    [CustomEditor(typeof(ElytopiaSDK))]
    public class ElytopiaSDKEditor : UnityEditor.Editor
    {
        private GameHubCommand selectedCommandToSend;
        private string valuesToSend = "";

        private GameHubCommand simulatedReceiveCommand;
        private string simulatedReceiveValue = "";

        private string lastReceivedEventType = "None";
        private string lastReceivedValue = "None";

        private void OnEnable()
        {
            var bridge = (ElytopiaSDK) target;
            bridge.OnReceiveEvent += OnHubEventReceived;
        }

        private void OnDisable()
        {
            var bridge = (ElytopiaSDK) target;
            bridge.OnReceiveEvent -= OnHubEventReceived;
        }

        private void OnHubEventReceived(GameHubCommand eventType, string value)
        {
            lastReceivedEventType = eventType.ToString();
            lastReceivedValue = value;
            Repaint();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("◄ Send Command To Hub", EditorStyles.boldLabel);
            selectedCommandToSend = (GameHubCommand) EditorGUILayout.EnumPopup("Command", selectedCommandToSend);
            valuesToSend = EditorGUILayout.TextField("Values", valuesToSend);

            if (GUILayout.Button("▶ Send To Hub"))
            {
                var bridge = (ElytopiaSDK) target;
                bridge.SendToHub(selectedCommandToSend, valuesToSend);
            }

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("◄ Simulate Receive From Hub", EditorStyles.boldLabel);
            simulatedReceiveCommand = (GameHubCommand) EditorGUILayout.EnumPopup("Simulated Command", simulatedReceiveCommand);
            simulatedReceiveValue = EditorGUILayout.TextField("Simulated Value", simulatedReceiveValue);

            if (GUILayout.Button("▶ Simulate Receive"))
            {
                var bridge = (ElytopiaSDK) target;
                var simulatedData = $"{simulatedReceiveCommand};{simulatedReceiveValue}";
                bridge.OnReceiveFromHub(simulatedData);
            }

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("◄ Last Received Event (Optional)", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Event Type:", lastReceivedEventType);
            EditorGUILayout.LabelField("Value:", lastReceivedValue);
        }
    }
}