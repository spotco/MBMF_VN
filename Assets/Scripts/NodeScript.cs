﻿using UnityEngine;
using System.Collections.Generic;

public abstract class NodeScriptEvent {
	public virtual void i_initialize(GameMain game, EventModal modal) {}
	public virtual void i_update(GameMain game, EventModal modal) {}
}

public class NodeScript {
	
	public int _id = 0;
	public string _title = "";
	public string _background = "";
	public string _music = "";
	public List<string> _requirement_items = new List<string>();
	public List<NodeScriptEvent> _events = new List<NodeScriptEvent>();
	public List<int> _links = new List<int>();
	public bool _affinity_requirement;
	
	public NodeScript i_initialize(TextAsset text) {
		JSONObject root;
		try {
			root = JSONObject.Parse(text.text);
		} catch (System.Exception e) {
			Debug.LogError(SPUtil.sprintf("JSON FILE(%s) INVALID",text.name));
			return this;
		}
		_id = (int) root.GetNumber("id");
		_title = root.GetString("title");
		_background = root.GetString("background");
		_music = root.GetString("music");
		_affinity_requirement = root.ContainsKey("affinityrequirement");
		
		JSONArray requirement_item_json = root.GetArray("requirementitem");
		for (int i = 0; i < requirement_item_json.Length; i++) {
			_requirement_items.Add(requirement_item_json[i].Str);
		}
		
		JSONArray event_json = root.GetArray("event");
		for (int i = 0; i < event_json.Length; i++) {
			JSONObject itr = event_json[i].Obj;
			string type = itr.GetString("type");
			NodeScriptEvent itr_neu = null;
			if (type == "showcharacter") {
				itr_neu = new NodeScriptEvent_ShowCharacter() {
					_character = itr.GetString("character"),
					_image = itr.GetString("image"),
					_xpos = (float) itr.GetNumber("xpos"),
					_xscale = (float) itr.GetNumber("xscale")
				};
				
			} else if (type == "changebackground") {
				itr_neu = new NodeScriptEvent_ChangeBackground() {
					_background = itr.GetString("background")
				};
				
			} else if (type == "dialogue") {
				itr_neu = new NodeScriptEvent_Dialogue() {
					_character = itr.ContainsKey("character") ? itr.GetString("character") : NodeScriptEvent_Dialogue.CHARACTER_NARRATOR,
					_text = itr.GetString("text"),
					_xpos = itr.ContainsKey("xpos") ? ((float)itr.GetNumber("xpos")) : 0,
					_ypos = itr.ContainsKey("ypos") ? ((float)itr.GetNumber("ypos")) : -130
				};
			
			} else if (type == "transitioncharacter") {
				itr_neu = new NodeScriptEvent_TransitionCharacter() {
					_character = itr.GetString("character"),
					_image = itr.GetString("image"),
					_xscale = (float) itr.GetNumber("xscale")
				};
			
			} else if (type == "movecharacter") {
				itr_neu = new NodeScriptEvent_MoveCharacter() {
					_character = itr.GetString("character"),
					_xto = (float) itr.GetNumber("xto")
				};
			
			} else if (type == "additem") {
				itr_neu = new NodeScriptEvent_AddItem() {
					_item = itr.GetString("item")
				};
			
			} else if (type == "removeitem") {
				itr_neu = new NodeScriptEvent_RemoveItem() {
					_item = itr.GetString("item")
				};
			
			} else if (type == "hidecharacter") {
				itr_neu = new NodeScriptEvent_HideCharacter() {
					_character = itr.GetString("character")
				};
				
			} else if (type == "addaffinity") {
				itr_neu = new NodeScriptEvent_AddAffinity();
				
			} else if (type == "rename") {
				itr_neu = new NodeScriptEvent_Rename() {
					_name_start = itr.GetString("namestart"),
					_name_end = itr.GetString("nameend")
				};
				
			} else {
				SPUtil.logf("unknown type %s",type);
			}
			
			if (itr_neu != null) {
				_events.Add(itr_neu);
			}
		}
		
		JSONArray links_json = root.GetArray("links");
		for (int i = 0; i < links_json.Length; i++) {
			_links.Add((int)links_json[i].Number);
		}
		
		return this;
	}
}
