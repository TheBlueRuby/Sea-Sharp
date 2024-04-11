using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Godot;

public class DialogEntry {
	public string stringType { get; set; }
	public string stringId { get; set; }
	public string iconPath { get; set; }
	public List<string> pages { get; set; } = new();
	public List<bool> pagesDone { get; set; } = new();

	public DialogEntry(string filePath, string type, string id) {
		stringType = type;
		stringId = id;

		JsonNode jsonData = GetEntryFromJson(filePath);

		iconPath = jsonData!["icon"]!.GetValue<string>();

		JsonArray pagesNode = jsonData!["pages"]!.AsArray();
		for (int i = 0; i < pagesNode.Count; i++) {
			string pagetext = pagesNode![i]!.GetValue<string>();
			pages.Add(pagetext);
			pagesDone.Add(false);
		}
	}

	public JsonNode GetEntryFromJson(string file) {
		string fileContents = FileAccess.Open(file, FileAccess.ModeFlags.Read).GetAsText();
		JsonNode fileAsJson = JsonNode.Parse(fileContents);
		JsonNode stringArray = fileAsJson![stringType];
		JsonNode stringEntry = null;

		for (int i = 0; i < stringArray.AsArray().Count; i++) {
			if (stringArray[i]!["id"]!.GetValue<string>() == stringId) {
				stringEntry = stringArray[i];
			}
		}
		if (stringEntry != null) {
			return stringEntry;
		} else {
			return null;
		}

	}
}
