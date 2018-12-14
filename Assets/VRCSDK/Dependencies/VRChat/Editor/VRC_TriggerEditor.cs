#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Text.RegularExpressions;

namespace VRCSDK2
{
    [CustomPropertyDrawer(typeof(VRCSDK2.VRC_Trigger.CustomTriggerTarget))]
    public class CustomTriggerTargetDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            if (property == null)
                return;

            SerializedProperty objectProperty = property.FindPropertyRelative("TriggerObject");
            SerializedProperty nameProperty = property.FindPropertyRelative("CustomName");

            EditorGUI.BeginProperty(rect, label, property);

            rect = EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive), label);
            Rect objectRect = new Rect(rect.x, rect.y, rect.width / 2 - 5, rect.height);
            Rect nameRect = new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, rect.height);

            VRC_Trigger current = null;
            if (objectProperty.objectReferenceValue != null)
                current = (objectProperty.objectReferenceValue as GameObject).GetComponent<VRC_Trigger>();
            current = EditorGUI.ObjectField(objectRect, current, typeof(VRC_Trigger), true) as VRC_Trigger;
            objectProperty.objectReferenceValue = current == null ? null : current.gameObject;
            
            VRC_EditorTools.CustomTriggerPopup(nameRect, objectProperty, nameProperty);

            EditorGUI.EndProperty();
        }
    }

    [CustomEditor(typeof(VRCSDK2.VRC_Trigger)), CanEditMultipleObjects]
    public class VRC_TriggerEditor : Editor
    {
        private static VRC_EventHandler.VrcBroadcastType[] unbufferedBroadcastTypes = new VRC_EventHandler.VrcBroadcastType[] { VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered, VRC_EventHandler.VrcBroadcastType.MasterUnbuffered, VRC_EventHandler.VrcBroadcastType.OwnerUnbuffered, VRC_EventHandler.VrcBroadcastType.Local };

        private ReorderableList[] eventLists = new ReorderableList[0];
        private ReorderableList[] relayLists = new ReorderableList[0];
        private ReorderableList[] objectLists = new ReorderableList[0];
        private bool[] visible = new bool[0];

        private SerializedProperty triggersProperty;
        private SerializedProperty proximityProperty;
        private SerializedProperty interactTextProperty;

        private VRC_Trigger.TriggerType addTriggerSelectedType = VRC_Trigger.TriggerType.Custom;

        public override void OnInspectorGUI()
        {
            triggersProperty = serializedObject.FindProperty("Triggers");
            proximityProperty = serializedObject.FindProperty("proximity");
            interactTextProperty = serializedObject.FindProperty("interactText");

            serializedObject.Update();

            SerializedProperty triggers = triggersProperty.Copy();
            int triggersLength = triggers.arraySize;

            if (eventLists.Length != triggersLength)
                eventLists = new ReorderableList[triggersLength];

            if (relayLists.Length != triggersLength)
                relayLists = new ReorderableList[triggersLength];

            if (objectLists.Length != triggersLength)
                objectLists = new ReorderableList[triggersLength];

            if (visible.Length != triggersLength)
            {
                bool[] newVisible = new bool[triggersLength];
                for (int idx = 0; idx < visible.Length && idx < newVisible.Length; ++idx)
                    newVisible[idx] = visible[idx];
                visible = newVisible;
            }

            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 30));
            RenderTriggers();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderTriggers()
        {
            GUIStyle triggerStyle = new GUIStyle(EditorStyles.helpBox);

            SerializedProperty triggers = triggersProperty.Copy();
            int triggersLength = triggers.arraySize;

            List<int> to_remove = new List<int>();
            for (int idx = 0; idx < triggersLength; ++idx)
            {
                SerializedProperty triggerProperty = triggers.GetArrayElementAtIndex(idx);
                SerializedProperty broadcastProperty = triggerProperty.FindPropertyRelative("BroadcastType");

                EditorGUILayout.BeginVertical(triggerStyle);

                if (RenderTriggerHeader(triggerProperty, ref visible[idx]))
                {
                    to_remove.Add(idx);
                    EditorGUILayout.EndVertical();

                    continue;
                }

                if (!visible[idx])
                {
                    EditorGUILayout.EndVertical();
                    continue;
                }

                if (!unbufferedBroadcastTypes.Contains((VRC_EventHandler.VrcBroadcastType)broadcastProperty.enumValueIndex) &&
                    ActiveEvents(triggerProperty).Any(e => e == VRC_EventHandler.VrcEventType.SendRPC))
                    EditorGUILayout.HelpBox("Consider using unbuffered broadcasts with RPCs.", MessageType.Error);

                EditorGUILayout.Separator();

                RenderTriggerEditor(triggerProperty, idx);

                if (eventLists.Length == triggersLength)
                {
                    EditorGUILayout.Separator();

                    if (triggerProperty.FindPropertyRelative("TriggerType").enumValueIndex != (int)VRC_Trigger.TriggerType.Relay)
                    {
                        RenderTriggerEventsEditor(triggerProperty, idx);

                        EditorGUILayout.Separator();

                        RenderTriggerAddObjectEvents(idx);

                        EditorGUILayout.Separator();
                    }
                }
                EditorGUILayout.EndVertical();
            }

            foreach (int idx in ((IEnumerable<int>)to_remove).Reverse())
                triggersProperty.Copy().DeleteArrayElementAtIndex(idx);

            RenderAddTrigger();
        }

        private void RenderTriggerEditor(SerializedProperty triggerProperty, int idx)
        {
            VRC_Trigger.TriggerType triggerType = (VRC_Trigger.TriggerType)triggerProperty.FindPropertyRelative("TriggerType").enumValueIndex;
            switch (triggerType)
            {
                case VRC_Trigger.TriggerType.Custom:
                    RenderCustom(triggerProperty);
                    break;
                case VRC_Trigger.TriggerType.Relay:
                    RenderRelay(triggerProperty, idx);
                    break;
                case VRC_Trigger.TriggerType.OnEnterTrigger:
                case VRC_Trigger.TriggerType.OnExitTrigger:
                case VRC_Trigger.TriggerType.OnEnterCollider:
                case VRC_Trigger.TriggerType.OnExitCollider:
                    RenderCollider(triggerProperty);
                    break;
                case VRC_Trigger.TriggerType.OnKeyDown:
                case VRC_Trigger.TriggerType.OnKeyUp:
                    RenderKey(triggerProperty);
                    break;
                case VRC_Trigger.TriggerType.OnTimer:
                    RenderTimer(triggerProperty);
                    break;
                case VRC_Trigger.TriggerType.OnDataStorageAdd:
                case VRC_Trigger.TriggerType.OnDataStorageChange:
                case VRC_Trigger.TriggerType.OnDataStorageRemove:
                    RenderDataStorage(triggerProperty);
                    break;
                default:
                    if (VRC_Trigger.TypeCollections.InteractiveTypes.Contains(triggerType) || VRC_Trigger.TypeCollections.PickupTypes.Contains(triggerType))
                        RenderInteractableEditor();
                    else
                        RenderEmpty(triggerProperty);
                    break;
            }
        }

        private List<VRC_Trigger.TriggerType> ActiveTypes
        {
            get
            {
                List<VRC_Trigger.TriggerType> activeTypes = new List<VRC_Trigger.TriggerType>();

                SerializedProperty triggers = triggersProperty.Copy();
                int triggersLength = triggers.arraySize;

                for (int idx = 0; idx < triggersLength; ++idx)
                {
                    VRC_Trigger.TriggerType triggerType = (VRC_Trigger.TriggerType)triggers.GetArrayElementAtIndex(idx).FindPropertyRelative("TriggerType").enumValueIndex;
                    activeTypes.Add(triggerType);
                }

                return activeTypes;
            }
        }

        private List<VRC_Trigger.TriggerType> HiddenTypes = new List<VRC_Trigger.TriggerType> { VRC_Trigger.TriggerType.OnDataStorageAdd, VRC_Trigger.TriggerType.OnDataStorageRemove };

        private List<VRC_EventHandler.VrcEventType> ActiveEvents(SerializedProperty triggerProperty)
        {
            List<VRC_EventHandler.VrcEventType> activeTypes = new List<VRC_EventHandler.VrcEventType>();

            SerializedProperty events = triggerProperty.FindPropertyRelative("Events").Copy();
            int eventsLength = events.arraySize;

            for (int idx = 0; idx < eventsLength; ++idx)
            {
                VRC_EventHandler.VrcEventType eventType = (VRC_EventHandler.VrcEventType)events.GetArrayElementAtIndex(idx).FindPropertyRelative("EventType").enumValueIndex;
                activeTypes.Add(eventType);
            }

            return activeTypes;
        }

        private void RenderAddTrigger()
        {
            Rect rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(15f));
            EditorGUILayout.Space();

            Rect selectedRect = new Rect(rect.x, rect.y, rect.width / 4 * 3 - 5, rect.height);
            Rect addRect = new Rect(selectedRect.x + selectedRect.width + 5, rect.y, rect.width / 4, rect.height);

            addTriggerSelectedType = VRC_EditorTools.FilteredEnumPopup(selectedRect, addTriggerSelectedType, v => (v == VRC_Trigger.TriggerType.Custom || ActiveTypes.All(t => t != v)) && (HiddenTypes.Contains(v) == false));

            if (GUI.Button(addRect, "Add"))
            {
                SerializedProperty triggersAry = triggersProperty;

                // hacks
                triggersAry.Next(true);
                triggersAry.Next(true);

                int triggersLength = triggersAry.intValue;
                triggersAry.intValue = triggersLength + 1;
                triggersAry.Next(true);

                for (int idx = 0; idx < triggersLength; ++idx)
                    triggersAry.Next(false);

                triggersAry.FindPropertyRelative("TriggerType").enumValueIndex = (int)addTriggerSelectedType;
                triggersAry.FindPropertyRelative("BroadcastType").enumValueIndex = (int)VRC_EventHandler.VrcBroadcastType.Always;
            }

            EditorGUILayout.EndHorizontal();
        }

        private bool RenderTriggerHeader(SerializedProperty triggerProperty, ref bool expand)
        {
            bool delete = false;

            if (!delete)
            {
                VRC_EventHandler.VrcBroadcastType? broadcast = null;

                Rect rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(15f));
                EditorGUILayout.Space();
                
                int baseWidth = (int)((rect.width - 35) / 3);
                Rect foldoutRect = new Rect(rect.x + 10, rect.y, 20, rect.height);
                Rect typeRect = new Rect(rect.x + 25, rect.y, baseWidth, rect.height);
                Rect broadcastRect = new Rect(rect.x + 30 + baseWidth, rect.y, baseWidth, rect.height);
                Rect removeRect = new Rect(rect.x + 35 + baseWidth * 2, rect.y, baseWidth, rect.height);

                expand = EditorGUI.Foldout(foldoutRect, expand, GUIContent.none);


                SerializedProperty triggerTypeProperty = triggerProperty.FindPropertyRelative("TriggerType");
                VRC_Trigger.TriggerType currentType = (VRC_Trigger.TriggerType)triggerTypeProperty.enumValueIndex;

                SerializedProperty nameProperty = triggerProperty.FindPropertyRelative("Name");
                if (string.IsNullOrEmpty(nameProperty.stringValue))
                    nameProperty.stringValue = "Unnamed";

                System.Func<string, string> rename = s => s == "Custom" ? s + " (" + nameProperty.stringValue + ")" : s;
                System.Func<VRC_Trigger.TriggerType, bool> predicate = v => (v == currentType || ActiveTypes.All(t => t != v)) && HiddenTypes.Contains(v) == false;

                triggerTypeProperty.enumValueIndex = (int)VRC_EditorTools.FilteredEnumPopup(typeRect, currentType, predicate, rename);
                currentType = (VRC_Trigger.TriggerType)triggerTypeProperty.enumValueIndex;

                if ((VRC_Trigger.TriggerType)triggerTypeProperty.enumValueIndex != VRC_Trigger.TriggerType.Relay)
                {
                    SerializedProperty broadcastTypeProperty = triggerProperty.FindPropertyRelative("BroadcastType");
                    broadcastTypeProperty.enumValueIndex = (int)(VRC_EventHandler.VrcBroadcastType)EditorGUI.EnumPopup(broadcastRect, (VRC_EventHandler.VrcBroadcastType)broadcastTypeProperty.enumValueIndex);
                    broadcast = (VRC_EventHandler.VrcBroadcastType)broadcastTypeProperty.enumValueIndex;
                }

                if (GUI.Button(removeRect, "Remove"))
                    delete = true;

                EditorGUILayout.EndHorizontal();

                if (broadcast.HasValue && expand)
                {
                    string message = null;
                    switch (broadcast.Value)
                    {
                        case VRC_EventHandler.VrcBroadcastType.Always:
                            message = "All are able to activate the trigger for everyone, and late-joiners will also trigger it.";
                            break;
                        case VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered:
                            message = "All are able to activate the trigger for everyone, but late-joiners will not trigger it.";
                            break;
                        case VRC_EventHandler.VrcBroadcastType.Local:
                            message = "All are able to activate the trigger for themselves only.";
                            break;
                        case VRC_EventHandler.VrcBroadcastType.Master:
                            message = "Only the Master is able to activate the trigger for everyone, and late-joiners will also trigger it.";
                            break;
                        case VRC_EventHandler.VrcBroadcastType.MasterUnbuffered:
                            message = "Only the Master is able to activate the trigger for everyone, but late-joiners will not trigger it.";
                            break;
                        case VRC_EventHandler.VrcBroadcastType.Owner:
                            message = "Only the Owner is able to activate the trigger for everyone, and late-joiners will also trigger it.";
                            break;
                        case VRC_EventHandler.VrcBroadcastType.OwnerUnbuffered:
                            message = "Only the Owner is able to activate the trigger for everyone, but late-joiners will not trigger it.";
                            break;
                    }
                    if (message != null)
                        EditorGUILayout.HelpBox(message, MessageType.Info);
                }
            }

            return delete;
        }

        private void RenderInteractableEditor()
        {
            EditorGUILayout.PropertyField(interactTextProperty, new GUIContent("Interaction Text"));
            proximityProperty.floatValue = EditorGUILayout.Slider("Proximity", proximityProperty.floatValue, 0f, 100f);
        }

        private void RenderTriggerAddObjectEvents(int triggerIdx)
        {
            GameObject importObject = null;
            importObject = EditorGUILayout.ObjectField("Generate Actions for ", importObject, typeof(GameObject), true) as GameObject;
            if (importObject != null)
                AddEventsForObject(importObject, triggerIdx);
        }

        private void RenderTriggerEventsEditor(SerializedProperty triggerProperty, int idx)
        {
            if (eventLists[idx] == null)
            {
                ReorderableList newList = new ReorderableList(serializedObject, triggerProperty.FindPropertyRelative("Events"), true, true, true, true);
                newList.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, "Actions");
                newList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty eventsListProperty = triggerProperty.FindPropertyRelative("Events");
                    SerializedProperty shadowListProperty = triggerProperty.FindPropertyRelative("DataStorageShadowValues");

                    if (shadowListProperty != null && shadowListProperty.arraySize != eventsListProperty.arraySize)
                        shadowListProperty.arraySize = eventsListProperty.arraySize;

                    SerializedProperty shadowProperty = shadowListProperty == null ? null : shadowListProperty.GetArrayElementAtIndex(index);
                    SerializedProperty eventProperty = eventsListProperty.GetArrayElementAtIndex(index);
                    SerializedProperty eventTypeProperty = eventProperty.FindPropertyRelative("EventType");
                    SerializedProperty parameterStringProperty = eventProperty.FindPropertyRelative("ParameterString");

                    string label = ((VRC_EventHandler.VrcEventType)eventTypeProperty.enumValueIndex).ToString();
                    if (!string.IsNullOrEmpty(parameterStringProperty.stringValue))
                        label += " (" + parameterStringProperty.stringValue + ")";

                    EditorGUI.LabelField(rect, label);
                    if (isFocused)
                        objectLists[idx] = null;
                    if (isActive)
                    {
                        EditorGUILayout.Space();

                        RenderEventEditor(shadowProperty, triggerProperty, eventProperty, idx);
                    }
                };
                newList.onAddDropdownCallback = (Rect buttonRect, ReorderableList list) =>
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (VRC_EventHandler.VrcEventType type in System.Enum.GetValues(typeof(VRC_EventHandler.VrcEventType)).Cast<VRC_EventHandler.VrcEventType>().Where(v => v != VRC_EventHandler.VrcEventType.SendMessage))
                    {
                        menu.AddItem(new GUIContent(type.ToString()), false, (t) => {
                            SerializedProperty eventsList = triggerProperty.FindPropertyRelative("Events");
                            eventsList.arraySize++;

                            SerializedProperty newEventProperty = eventsList.GetArrayElementAtIndex(eventsList.arraySize - 1);
                            newEventProperty.FindPropertyRelative("EventType").enumValueIndex = (int)(VRC_EventHandler.VrcEventType)t;
                            newEventProperty.FindPropertyRelative("ParameterObjects").arraySize = 0;

                            serializedObject.ApplyModifiedProperties();
                        }, type);
                    }
                    menu.ShowAsContext();

                    eventLists = new ReorderableList[0];
                    objectLists = new ReorderableList[0];
                    relayLists = new ReorderableList[0];
                };

                eventLists[idx] = newList;
            }

            ReorderableList eventList = eventLists[idx];
            eventList.DoLayoutList();
        }

        private void RenderDataStorage(SerializedProperty triggerProperty)
        {
            if (triggerProperty.serializedObject.targetObjects.Any(obj => (obj as VRC_Trigger).gameObject.GetComponent<VRC_DataStorage>() == null))
                EditorGUILayout.HelpBox("Data Storage Triggers require a VRC_DataStorage Component.", MessageType.Warning);
            else
            {
                SerializedProperty idxProperty = triggerProperty.FindPropertyRelative("DataElementIdx");
                VRC_DataStorage ds = (target as VRC_Trigger).gameObject.GetComponent<VRC_DataStorage>();

                if (ds.data == null)
                {
                    ds.data = new VRC_DataStorage.VrcDataElement[0];
                    idxProperty.intValue = -1;
                }

                List<string> names = new List<string>();
                names.Add("Any Data Element");
                foreach (VRC_DataStorage.VrcDataElement element in ds.data)
                    names.Add(element.name);

                int selectedIdx = idxProperty.intValue;
                if (selectedIdx == -1)
                    selectedIdx = 0;
                else
                    selectedIdx += 1;

                selectedIdx = EditorGUILayout.Popup("Data Element", selectedIdx, names.ToArray());

                if (selectedIdx == 0)
                    idxProperty.intValue = -1;
                else
                    idxProperty.intValue = selectedIdx - 1;
            }
        }

        private void RenderKey(SerializedProperty triggerProperty)
        {
            EditorGUILayout.PropertyField(triggerProperty.FindPropertyRelative("Key"));
        }

        private void RenderCollider(SerializedProperty triggerProperty)
        {
            EditorGUILayout.PropertyField(triggerProperty.FindPropertyRelative("TriggerIndividuals"));
            EditorGUILayout.PropertyField(triggerProperty.FindPropertyRelative("Layers"));
        }

        private void RenderTimer(SerializedProperty triggerProperty)
        {
            EditorGUILayout.PropertyField(triggerProperty.FindPropertyRelative("Repeat"));
            EditorGUILayout.PropertyField(triggerProperty.FindPropertyRelative("ResetOnEnable"));
            EditorGUILayout.PropertyField(triggerProperty.FindPropertyRelative("LowPeriodTime"));
            EditorGUILayout.PropertyField(triggerProperty.FindPropertyRelative("HighPeriodTime"));
        }

        private void RenderCustom(SerializedProperty triggerProperty)
        {
            SerializedProperty nameProperty = triggerProperty.FindPropertyRelative("Name");
            EditorGUILayout.PropertyField(triggerProperty.FindPropertyRelative("Name"));

            if (string.IsNullOrEmpty(nameProperty.stringValue))
                nameProperty.stringValue = "Unnamed";
        }

        private void RenderRelay(SerializedProperty triggerProperty, int idx)
        {
            if (relayLists[idx] == null)
            {
                ReorderableList newList = new ReorderableList(serializedObject, triggerProperty.FindPropertyRelative("Others"), true, true, true, true);
                newList.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, new GUIContent("Targets"));
                newList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty target = newList.serializedProperty.GetArrayElementAtIndex(index);

                    EditorGUI.PropertyField(rect, target, GUIContent.none);

                    target.serializedObject.ApplyModifiedProperties();
                };
                relayLists[idx] = newList;
            }
            relayLists[idx].DoLayoutList();
        }

        private void RenderEmpty(SerializedProperty triggerProperty)
        {
        }

        private void RenderTargetGameObjectList(SerializedProperty objectsProperty, int idx)
        {
            if (objectLists[idx] == null)
            {
                objectLists[idx] = new ReorderableList(objectsProperty.serializedObject, objectsProperty, true, true, true, true);
                objectLists[idx].drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, new GUIContent("Receivers"));
                objectLists[idx].drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty target = objectLists[idx].serializedProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, target, GUIContent.none);
                    target.serializedObject.ApplyModifiedProperties();
                };
            }
            objectLists[idx].DoLayoutList();
        }

        private void RenderTargetComponentList<T>(SerializedProperty objectsProperty, int idx) where T : Component
        {
            if (objectLists[idx] == null)
            {
                objectLists[idx] = new ReorderableList(objectsProperty.serializedObject, objectsProperty, true, true, true, true);
                objectLists[idx].drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, new GUIContent("Receivers"));
                objectLists[idx].drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty target = objectLists[idx].serializedProperty.GetArrayElementAtIndex(index);

                    T current = null;
                    if (target.objectReferenceValue != null)
                        current = (target.objectReferenceValue as GameObject).GetComponent<T>();

                    current = EditorGUI.ObjectField(rect, current, typeof(T), true) as T;
                    target.objectReferenceValue = current == null ? null : current.gameObject;
                    target.serializedObject.ApplyModifiedProperties();
                };
            }
            objectLists[idx].DoLayoutList();
        }

        public void RenderEventEditor(SerializedProperty shadowProperty, SerializedProperty triggerProperty, SerializedProperty eventProperty, int triggerIdx)
        {
            SerializedProperty eventTypeProperty = eventProperty.FindPropertyRelative("EventType");
            SerializedProperty parameterObjectProperty = eventProperty.FindPropertyRelative("ParameterObject");
            SerializedProperty parameterObjectsProperty = eventProperty.FindPropertyRelative("ParameterObjects");
            SerializedProperty parameterStringProperty = eventProperty.FindPropertyRelative("ParameterString");
            SerializedProperty parameterBoolOpProperty = eventProperty.FindPropertyRelative("ParameterBoolOp");
            SerializedProperty parameterFloatProperty = eventProperty.FindPropertyRelative("ParameterFloat");
            SerializedProperty parameterIntProperty = eventProperty.FindPropertyRelative("ParameterInt");

            if (parameterObjectProperty.objectReferenceValue != null && parameterObjectsProperty.arraySize == 0)
            {
                parameterObjectsProperty.arraySize = 1;
                parameterObjectsProperty.GetArrayElementAtIndex(0).objectReferenceValue = parameterObjectProperty.objectReferenceValue;
            }
            parameterObjectProperty.objectReferenceValue = null;

            switch ((VRC_EventHandler.VrcEventType)eventTypeProperty.enumValueIndex)
            {
                case VRCSDK2.VRC_EventHandler.VrcEventType.AnimationBool:
                    {
                        RenderTargetComponentList<Animator>(parameterObjectsProperty, triggerIdx);

                        RenderPropertyEditor(shadowProperty, parameterStringProperty, new GUIContent("Variable"));

                        RenderPropertyEditor(shadowProperty, parameterBoolOpProperty, new GUIContent("Operation"), true);
                        break;
                    }
                case VRCSDK2.VRC_EventHandler.VrcEventType.AnimationFloat:
                    {
                        RenderTargetComponentList<Animator>(parameterObjectsProperty, triggerIdx);

                        RenderPropertyEditor(shadowProperty, parameterStringProperty, new GUIContent("Variable"));
                        RenderPropertyEditor(shadowProperty, parameterFloatProperty, new GUIContent("Value"));
                        break;
                    }
                case VRCSDK2.VRC_EventHandler.VrcEventType.AnimationTrigger:
                    {
                        RenderTargetComponentList<Animator>(parameterObjectsProperty, triggerIdx);

                        RenderPropertyEditor(shadowProperty, parameterStringProperty, new GUIContent("Trigger"));
                        break;
                    }
                case VRCSDK2.VRC_EventHandler.VrcEventType.PlayAnimation:
                    {
                        RenderTargetComponentList<Animation>(parameterObjectsProperty, triggerIdx);

                        RenderPropertyEditor(shadowProperty, parameterStringProperty, new GUIContent("Clip"));

                        for (int idx = 0; idx < parameterObjectsProperty.arraySize; ++idx)
                        {
                            GameObject obj = parameterObjectsProperty.GetArrayElementAtIndex(idx).objectReferenceValue as GameObject;
                            Animation anim = obj == null ? null : obj.GetComponent<Animation>();
                            if (anim != null && anim.GetClip(parameterStringProperty.stringValue) == null)
                            {
                                EditorGUILayout.HelpBox("Could not locate " + parameterStringProperty.stringValue + " on " + obj.name + "; is it legacy?", MessageType.Error);
                                break;
                            }
                        }
                        break;
                    }
                case VRCSDK2.VRC_EventHandler.VrcEventType.AudioTrigger:
                    {
                        RenderTargetComponentList<AudioSource>(parameterObjectsProperty, triggerIdx);

                        List<string> clipNames = new List<string>();
                        if (parameterObjectsProperty.arraySize > 0)
                        {
                            int idx = 0;
                            for (; idx < parameterObjectsProperty.arraySize; ++idx)
                            {
                                SerializedProperty prop = parameterObjectsProperty.GetArrayElementAtIndex(0);
                                GameObject obj = prop.objectReferenceValue != null ? prop.objectReferenceValue as GameObject : null;
                                if (obj != null)
                                {
                                    foreach (AudioSource source in obj.GetComponents<AudioSource>())
                                        if (source.clip != null && !string.IsNullOrEmpty(source.clip.name))
                                            clipNames.Add(source.clip.name);
                                    break;
                                }
                            }
                            for (; idx < parameterObjectsProperty.arraySize; ++idx)
                            {
                                SerializedProperty prop = parameterObjectsProperty.GetArrayElementAtIndex(0);
                                GameObject obj = prop.objectReferenceValue != null ? prop.objectReferenceValue as GameObject : null;
                                if (obj != null)
                                {
                                    List<string> thisClipNames = new List<string>();
                                    foreach (AudioSource source in obj.GetComponents<AudioSource>())
                                        if (source.clip != null && !string.IsNullOrEmpty(source.clip.name))
                                            thisClipNames.Add(source.clip.name);
                                    clipNames.RemoveAll(s => thisClipNames.Contains(s) == false);
                                }
                            }
                        }

                        clipNames.Insert(0, "");
                        int selectedIdx;
                        for (selectedIdx = clipNames.Count - 1; selectedIdx > 0; --selectedIdx)
                            if (parameterStringProperty.stringValue == clipNames[selectedIdx])
                                break;

                        parameterStringProperty.stringValue = clipNames[EditorGUILayout.Popup("Clip", selectedIdx, clipNames.ToArray())];
                        break;
                    }
                case VRCSDK2.VRC_EventHandler.VrcEventType.MeshVisibility:
                    {
                        RenderTargetComponentList<Renderer>(parameterObjectsProperty, triggerIdx);
                        RenderPropertyEditor(shadowProperty, parameterBoolOpProperty, new GUIContent("Operation"), true);
                        break;
                    }
                case VRCSDK2.VRC_EventHandler.VrcEventType.RunConsoleCommand:
                    {
                        RenderPropertyEditor(shadowProperty, parameterStringProperty, new GUIContent("Command"));
                        break;
                    }
                case VRCSDK2.VRC_EventHandler.VrcEventType.SendMessage:
                    {
                        RenderTargetGameObjectList(parameterObjectsProperty, triggerIdx);
                        if (parameterObjectsProperty.arraySize > 0)
                            RenderMethodSelector(eventProperty);
                    }
                    break;
                case VRCSDK2.VRC_EventHandler.VrcEventType.SetGameObjectActive:
                    {
                        RenderTargetGameObjectList(parameterObjectsProperty, triggerIdx);
                        RenderPropertyEditor(shadowProperty, parameterBoolOpProperty, new GUIContent("Operation"), true);
                        break;
                    }
                case VRCSDK2.VRC_EventHandler.VrcEventType.SetParticlePlaying:
                    {
                        RenderTargetGameObjectList(parameterObjectsProperty, triggerIdx);
                        RenderPropertyEditor(shadowProperty, parameterBoolOpProperty, new GUIContent("Operation"), true);
                        break;
                    }
                case VRCSDK2.VRC_EventHandler.VrcEventType.TeleportPlayer:
                    {
                        RenderTargetGameObjectList(parameterObjectsProperty, triggerIdx);
                        break;
                    }
                case VRCSDK2.VRC_EventHandler.VrcEventType.SetWebPanelURI:
                    {
                        RenderTargetComponentList<VRCSDK2.VRC_WebPanel>(parameterObjectsProperty, triggerIdx);
                        RenderPropertyEditor(shadowProperty, parameterStringProperty, new GUIContent("URI"));
                        break;
                    }
                case VRCSDK2.VRC_EventHandler.VrcEventType.SetWebPanelVolume:
                    {
                        RenderTargetComponentList<VRCSDK2.VRC_WebPanel>(parameterObjectsProperty, triggerIdx);
                        parameterFloatProperty.floatValue = EditorGUILayout.Slider("Volume", parameterFloatProperty.floatValue, 0f, 1f);
                        break;
                    }
                case VRCSDK2.VRC_EventHandler.VrcEventType.SendRPC:
                    {
                        RenderTargetGameObjectList(parameterObjectsProperty, triggerIdx);

                        if (parameterObjectsProperty.arraySize > 0)
                        {
                            RenderMethodSelector(eventProperty);
                            RenderParameterEditor(eventProperty);
                        }
                    }
                    break;
                case VRC_EventHandler.VrcEventType.ActivateCustomTrigger:
                    {
                        RenderTargetComponentList<VRC_Trigger>(parameterObjectsProperty, triggerIdx);

                        VRC_EditorTools.CustomTriggerPopup("Name", parameterObjectsProperty, parameterStringProperty);
                    }
                    break;
                case VRC_EventHandler.VrcEventType.SpawnObject:
                    {
                        VRC_SceneDescriptor scene = FindObjectOfType<VRC_SceneDescriptor>();

                        string path = parameterStringProperty.stringValue;
                        GameObject found = scene != null ? scene.DynamicPrefabs.FirstOrDefault(p => AssetDatabase.GetAssetOrScenePath(p) == path) : null;
                        found = found == null ? AssetDatabase.LoadAssetAtPath<GameObject>(path) : found;
                        GameObject newFound = EditorGUILayout.ObjectField("Prefab", found, typeof(GameObject), false) as GameObject;
                        parameterStringProperty.stringValue = newFound == null ? null : AssetDatabase.GetAssetOrScenePath(newFound);

                        if (scene != null && found != newFound)
                        {
                            scene.DynamicPrefabs.Add(newFound);
                            scene.DynamicPrefabs.Remove(found);
                        }
                    }
                    break;
                case VRC_EventHandler.VrcEventType.DestroyObject:
                    {
                        SerializedProperty broadcastTypeProperty = triggerProperty.FindPropertyRelative("BroadcastType");
                        VRC_EventHandler.VrcBroadcastType broadcast = (VRC_EventHandler.VrcBroadcastType)broadcastTypeProperty.enumValueIndex;
                        if (broadcast != VRC_EventHandler.VrcBroadcastType.Always && broadcast != VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered)
                            EditorGUILayout.HelpBox("Not all clients will destroy the object.", MessageType.Warning);

                        RenderTargetGameObjectList(parameterObjectsProperty, triggerIdx);
                    }
                    break;
                case VRC_EventHandler.VrcEventType.SetLayer:
                    {
                        RenderTargetGameObjectList(parameterObjectsProperty, triggerIdx);
                        parameterIntProperty.intValue = (int)EditorGUILayout.LayerField("Layer", parameterIntProperty.intValue);
                    }
                    break;
                case VRC_EventHandler.VrcEventType.SetMaterial:
                    {
                        RenderTargetGameObjectList(parameterObjectsProperty, triggerIdx);

                        VRC_SceneDescriptor scene = FindObjectOfType<VRC_SceneDescriptor>();

                        string path = parameterStringProperty.stringValue;
                        Material found = scene != null ? scene.DynamicMaterials.FirstOrDefault(p => AssetDatabase.GetAssetOrScenePath(p) == path) : null;
                        found = found == null ? AssetDatabase.LoadAssetAtPath<Material>(path) : found;
                        Material newFound = EditorGUILayout.ObjectField("Material", found, typeof(Material), false) as Material;
                        parameterStringProperty.stringValue = newFound == null ? null : AssetDatabase.GetAssetOrScenePath(newFound);

                        if (scene != null && found != newFound)
                        {
                            scene.DynamicMaterials.Add(newFound);
                            scene.DynamicMaterials.Remove(found);
                        }
                    }
                    break;
                case VRC_EventHandler.VrcEventType.AddDamage:
                    {
                        RenderTargetGameObjectList(parameterObjectsProperty, triggerIdx);
                        RenderPropertyEditor(shadowProperty, parameterFloatProperty, new GUIContent("Damage"));
                    }
                    break;
                case VRC_EventHandler.VrcEventType.AddHealth:
                    {
                        RenderTargetGameObjectList(parameterObjectsProperty, triggerIdx);
                        RenderPropertyEditor(shadowProperty, parameterFloatProperty, new GUIContent("Health"));
                    }
                    break;
                default:
                    EditorGUILayout.HelpBox("Unsupported event type", MessageType.Error);
                    break;
            }            
        }

        void RenderParameterEditor(SerializedProperty eventProperty)
        {
            EditorGUI.BeginChangeCheck();

            SerializedProperty parameterIntProperty = eventProperty.FindPropertyRelative("ParameterInt");
            SerializedProperty parameterObjectsProperty = eventProperty.FindPropertyRelative("ParameterObjects");
            SerializedProperty parameterStringProperty = eventProperty.FindPropertyRelative("ParameterString");
            SerializedProperty parameterBytesProperty = eventProperty.FindPropertyRelative("ParameterBytes");

            parameterIntProperty.intValue = (int)(VRCSDK2.VRC_EventHandler.VrcTargetType)EditorGUILayout.EnumPopup("Targets", (VRCSDK2.VRC_EventHandler.VrcTargetType)parameterIntProperty.intValue);

            string message = null;
            switch((VRCSDK2.VRC_EventHandler.VrcTargetType)parameterIntProperty.intValue)
            {
                case VRC_EventHandler.VrcTargetType.All:
                    message = "Will execute on all clients, except for those that join later.";
                    break;
                case VRC_EventHandler.VrcTargetType.AllBuffered:
                    message = "Will execute on all clients, including those that join later.";
                    break;
                case VRC_EventHandler.VrcTargetType.Local:
                    message = "Will execute locally.";
                    break;
                case VRC_EventHandler.VrcTargetType.Master:
                    message = "Will execute on the Master.";
                    break;
                case VRC_EventHandler.VrcTargetType.Others:
                    message = "Will execute for others but not locally, except for those that join later.";
                    break;
                case VRC_EventHandler.VrcTargetType.OthersBuffered:
                    message = "Will execute for others but not locally, ncluding those that join later.";
                    break;
                case VRC_EventHandler.VrcTargetType.Owner:
                    message = "Will execute on the Owner.";
                    break;
            }
            if (message != null)
                EditorGUILayout.HelpBox(message, MessageType.Info);

            Dictionary<string, List<MethodInfo>> methods = VRC_EditorTools.GetSharedPublicMethodsOnGameObjects(parameterObjectsProperty);
            if (methods.Count == 0)
            {
                EditorGUILayout.HelpBox("No applicable methods found.", MessageType.Error);
                return;
            }

            List<MethodInfo> methodInfos = methods.Values.Aggregate(new List<MethodInfo>(), (acc, lst) => { acc.AddRange(lst); return acc; });

            MethodInfo info = methodInfos.FirstOrDefault(m => m.Name == parameterStringProperty.stringValue);
            if (info == null)
                return;

            ParameterInfo[] paramInfo = info.GetParameters();

            object[] parameters = VRCSDK2.VRC_Serialization.ParameterDecoder(VRC_EditorTools.ReadBytesFromProperty(parameterBytesProperty));
            if (parameters == null)
                parameters = new object[paramInfo.Length];
            if (parameters.Length != paramInfo.Length)
                Array.Resize(ref parameters, paramInfo.Length);

            bool finalIntAsPlayerID = false;

            for (int idx = 0; idx < parameters.Length; ++idx)
            {
                Type paramType = paramInfo[idx].ParameterType;
                if (paramType == typeof(Color))
                {
                    if (parameters[idx] == null || parameters[idx].GetType() != paramType)
                        parameters[idx] = Color.black;
                    parameters[idx] = EditorGUILayout.ColorField(paramInfo[idx].Name, (Color)parameters[idx]);
                }
                else if (paramType == typeof(bool))
                {
                    if (parameters[idx] == null || parameters[idx].GetType() != paramType)
                        parameters[idx] = false;
                    parameters[idx] = EditorGUILayout.Toggle(paramInfo[idx].Name, (bool)parameters[idx]);
                }
                else if (paramType == typeof(double))
                {
                    if (parameters[idx] == null || parameters[idx].GetType() != paramType)
                        parameters[idx] = 0d;
                    parameters[idx] = EditorGUILayout.DoubleField(paramInfo[idx].Name, (double)parameters[idx]);
                }
                else if (paramType == typeof(float))
                {
                    if (parameters[idx] == null || parameters[idx].GetType() != paramType)
                        parameters[idx] = 0f;
                    parameters[idx] = EditorGUILayout.FloatField(paramInfo[idx].Name, (float)parameters[idx]);
                }
                else if (paramType == typeof(int))
                {
                    if (idx == parameters.Length - 1)
                        finalIntAsPlayerID = EditorGUILayout.Toggle("Use Player ID as last", finalIntAsPlayerID || parameters[idx] == null);
                    if (!finalIntAsPlayerID)
                    {
                        if (parameters[idx] == null || parameters[idx].GetType() != paramType)
                            parameters[idx] = 0;
                        parameters[idx] = EditorGUILayout.IntField(paramInfo[idx].Name, (int)parameters[idx]);
                    }
                }
                else if (paramType == typeof(long))
                {
                    if (parameters[idx] == null || parameters[idx].GetType() != paramType)
                        parameters[idx] = 0l;
                    parameters[idx] = EditorGUILayout.LongField(paramInfo[idx].Name, (long)parameters[idx]);
                }
                else if (paramType == typeof(UnityEngine.Rect))
                {
                    if (parameters[idx] == null || parameters[idx].GetType() != paramType)
                        parameters[idx] = new Rect();
                    parameters[idx] = EditorGUILayout.RectField(paramInfo[idx].Name, (UnityEngine.Rect)parameters[idx]);
                }
                else if (paramType == typeof(string))
                {
                    if (parameters[idx] == null || parameters[idx].GetType() != paramType)
                        parameters[idx] = "";
                    parameters[idx] = EditorGUILayout.TextField(paramInfo[idx].Name, (string)parameters[idx]);
                }
                else if (paramType == typeof(Vector2))
                {
                    if (parameters[idx] == null || parameters[idx].GetType() != paramType)
                        parameters[idx] = new Vector2();
                    parameters[idx] = EditorGUILayout.Vector2Field(paramInfo[idx].Name, (Vector2)parameters[idx]);
                }
                else if (paramType == typeof(Vector3))
                {
                    if (parameters[idx] == null || parameters[idx].GetType() != paramType)
                        parameters[idx] = new Vector3();
                    parameters[idx] = EditorGUILayout.Vector3Field(paramInfo[idx].Name, (Vector3)parameters[idx]);
                }
                else if (paramType == typeof(Vector4))
                {
                    if (parameters[idx] == null || parameters[idx].GetType() != paramType)
                        parameters[idx] = new Vector4();
                    parameters[idx] = EditorGUILayout.Vector4Field(paramInfo[idx].Name, (Vector4)parameters[idx]);
                }
                else if (paramType == typeof(UnityEngine.Object) || paramType.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    if (parameters[idx] != null && parameters[idx].GetType() != paramType)
                        parameters[idx] = null;
                    parameters[idx] = EditorGUILayout.ObjectField(paramInfo[idx].Name, (UnityEngine.Object)parameters[idx], paramType, true);
                }
                else
                    EditorGUILayout.LabelField("Unable to handle " + paramType.Name, EditorStyles.boldLabel);
            }

            if (finalIntAsPlayerID)
                Array.Resize(ref parameters, parameters.Length - 1);

            VRC_EditorTools.WriteBytesToProperty(parameterBytesProperty, VRCSDK2.VRC_Serialization.ParameterEncoder(parameters));
        }

        void RenderMethodSelector(SerializedProperty eventProperty)
        {
            SerializedProperty parameterObjectsProperty = eventProperty.FindPropertyRelative("ParameterObjects");
            SerializedProperty parameterStringProperty = eventProperty.FindPropertyRelative("ParameterString");

            Dictionary<string, List<MethodInfo>> methods = VRC_EditorTools.GetSharedPublicMethodsOnGameObjects(parameterObjectsProperty);
            if (methods.Count == 0)
                return;

            List<string> combined = methods
                .Select(pair => pair.Value.Select(s => pair.Key + "." + s.Name))
                .Aggregate((a, b) =>
                {
                    var v = new List<string>();
                    v.AddRange(a);
                    v.AddRange(b);
                    return v;
                }).ToList();
            combined.Insert(0, "Custom Method");

            int currentIndex = string.IsNullOrEmpty(parameterStringProperty.stringValue) ? 0 : combined.FindIndex(s =>
            {
                var split = s.Split('.');
                return split.Length > 1 && s.Split('.')[1] == parameterStringProperty.stringValue;
            });
            if (currentIndex < 0 || currentIndex >= combined.Count)
                currentIndex = 0;

            int newIndex = EditorGUILayout.Popup("Method", currentIndex, combined.ToArray());
            if (newIndex != currentIndex)
                parameterStringProperty.stringValue = "";
            currentIndex = newIndex;

            if (currentIndex == 0)
                EditorGUILayout.PropertyField(parameterStringProperty, new GUIContent("Custom Method"));
            else
                parameterStringProperty.stringValue = combined[currentIndex].Split('.')[1];
        }

        private void RenderPropertyEditor(SerializedProperty shadowProperty, SerializedProperty property, GUIContent label, bool isBoolOp = false)
        {
            VRC_DataStorage ds = (target as VRC_Trigger).gameObject.GetComponent<VRC_DataStorage>();
            if (ds == null || ds.data == null || ds.data.Length == 0 || shadowProperty == null)
            {
                if (isBoolOp)
                    VRC_EditorTools.FilteredEnumPopup<VRCSDK2.VRC_EventHandler.VrcBooleanOp>(label.text, property, s => s != VRC_EventHandler.VrcBooleanOp.Unused);
                else
                    EditorGUILayout.PropertyField(property, label);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);

            bool renderField = false;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    {
                        SerializedProperty prop = shadowProperty.FindPropertyRelative("ParameterBoolOp");
                        renderField = !DataStorageElementPopup(ds, VRC_DataStorage.VrcDataType.Bool, prop);                        
                    }
                    break;
                case SerializedPropertyType.Float:
                    {
                        SerializedProperty prop = shadowProperty.FindPropertyRelative("ParameterFloat");
                        renderField = !DataStorageElementPopup(ds, VRC_DataStorage.VrcDataType.Float, prop);
                    }
                    break;
                case SerializedPropertyType.Integer:
                    {
                        SerializedProperty prop = shadowProperty.FindPropertyRelative("ParameterInt");
                        renderField = !DataStorageElementPopup(ds, VRC_DataStorage.VrcDataType.Int, prop);
                    }
                    break;
                case SerializedPropertyType.String:
                    {
                        SerializedProperty prop = shadowProperty.FindPropertyRelative("ParameterString");
                        renderField = !DataStorageElementPopup(ds, VRC_DataStorage.VrcDataType.String, prop);
                    }
                    break;
                default:
                    {
                        renderField = true;
                    }
                    break;
            }

            if (renderField)
                EditorGUILayout.PropertyField(property, GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }

        private bool DataStorageElementPopup(VRC_DataStorage ds, VRC_DataStorage.VrcDataType type, SerializedProperty prop)
        {
            List<string> vals = ds.data.Where(el => el.type == type).Select(el => el.name).ToList();
            if (vals.Count == 0)
                return false;

            vals.Insert(0, "Custom");

            int selectedIdx = prop.stringValue == null ? 0 : vals.IndexOf(prop.stringValue);
            if (selectedIdx < 0 || selectedIdx > vals.Count)
                selectedIdx = 0;

            int idx = EditorGUILayout.Popup(selectedIdx, vals.ToArray());
            if (idx == 0)
            {
                prop.stringValue = null;
                return false;
            }
            else
            {
                prop.stringValue = vals[idx];
                return true;
            }
        }

        private void AddEventsForObject(GameObject obj, int triggerIdx)
        {
            if (targets == null)
                return;

            VRCSDK2.IVRCEventProvider[] providers = obj.GetComponents<VRCSDK2.IVRCEventProvider>();
            List<VRC_EventHandler.VrcEvent> newEvents = new List<VRC_EventHandler.VrcEvent>();
            foreach (VRCSDK2.IVRCEventProvider provider in providers)
                newEvents.AddRange(provider.ProvideEvents());

            (target as VRC_Trigger).Triggers[triggerIdx].Events.AddRange(newEvents);

            serializedObject.CopyFromSerializedProperty(new SerializedObject(target).FindProperty("Triggers"));
        }
    }
}
#endif