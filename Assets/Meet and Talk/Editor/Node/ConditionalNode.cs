using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using MeetAndTalk.Editor;
using System;
using System.Collections.Generic;

namespace MeetAndTalk.Nodes
{
    [System.Serializable]
    public class ConditionalNode : BaseNode
    {
        private ReqFlag requiredFlags;
        public ReqFlag RequiredFlags { get => requiredFlags; set => requiredFlags = value; }
        private ObjectField requiredFlags_Field;

        public List<DialogueNodePort> dialogueNodePorts = new List<DialogueNodePort>();

        public ConditionalNode() { }

        public ConditionalNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            // Assign references to the editor window and graph view for later use.
            editorWindow = _editorWindow;
            graphView = _graphView;

            title = "Conditional";
            SetPosition(new Rect(_position, defualtNodeSize));
            nodeGuid = Guid.NewGuid().ToString();

            // Add Better Title
            GenerateBetterTitle("Conditional", "Basic Node to do a conditional branch", "Icon/NodeIcon/Branch");

            AddInputPort("Input", Port.Capacity.Multi);
            AddOutputPort("True Output", Port.Capacity.Single);
            AddOutputPort("False Output", Port.Capacity.Single);

            // RequiredFlags
            requiredFlags_Field = new ObjectField("Required Flags")
            {
                objectType = typeof(ReqFlag),
                allowSceneObjects = true,
            };
            requiredFlags_Field.RegisterValueChangedCallback(value =>
            {
                requiredFlags = value.newValue as ReqFlag;
                //UpdatePortraits();
            });
            requiredFlags_Field.SetValueWithoutNotify(requiredFlags);
            mainContainer.Add(requiredFlags_Field);

            // Refresh the node's state and UI.
            RefreshExpandedState();         // Update the expanded/collapsed state of the node.
            RefreshPorts();                 // Ensure all ports are correctly configured and displayed.
            AddValidationContainer();       // Add containers for error and warning messages to the node.
        }

        public override void ReloadLanguage()
        {
            //N/A
        }

        public override void LoadValueInToField()
        {
            requiredFlags_Field.SetValueWithoutNotify(requiredFlags);
        }

        public static ConditionalNode CreateNewGraphNode(Vector2 _position, DialogueEditorWindow _editorWindow, DialogueGraphView _graphView)
        {
            var tmp = new ConditionalNode(_position, _editorWindow, _graphView);
            tmp.name = "Conditional";
            tmp.ReloadLanguage();

            return tmp; // Return the newly created node.
        }

        public static ConditionalNode GenerateNode(ConditionalNodeData data, DialogueEditorWindow editor, DialogueGraphView graph)
        {
            // Generate a new DialogueNode using its factory method.
            ConditionalNode newNode = ConditionalNode.CreateNewGraphNode(data.Position, editor, graph);

            // Restore the node's unique identifier and values.
            newNode.nodeGuid = data.NodeGuid;

            newNode.requiredFlags = data.RequiredGameFlagCombo;

            // Load the restored values into the corresponding fields.
            newNode.LoadValueInToField();

            return newNode; // Return the generated node.
        }

        public ConditionalNodeData SaveNodeData()
        {
            var nodeData = new ConditionalNodeData
            {
                // Base Settings
                NodeGuid = nodeGuid,
                Position = GetPosition().position,

                //required flags
                RequiredGameFlagCombo = requiredFlags,
            };

            return nodeData; // Return the serialized data.
        }

        public override void SetValidation()
        {
            List<string> error = new List<string>();
            List<string> warning = new List<string>();

            Port input = inputContainer.Query<Port>().First();
            if (!input.connected) warning.Add("Node cannot be called");

            Port output = outputContainer.Query<Port>().First();
            if (!output.connected) error.Add("Output does not lead to any node");

            ErrorList = error;
            WarningList = warning;
        }
    }
}