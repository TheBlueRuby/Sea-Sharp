using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Godot;

namespace SeaSharp {
	public class DialogEntry {
		public string StringType { get; set; }
		public string StringId { get; set; }
		public string IconPath { get; set; }
		public List<string> Pages { get; set; } = new();
		public List<bool> PagesDone { get; set; } = new();

		/// <summary>
		/// Constructor for DialogEntry class that initializes its properties based on the provided parameters and JSON data.
		/// </summary>
		/// <param name="filePath">The path of the JSON file to load</param>
		/// <param name="type">The type of string (e.g <c>items</c></param>
		/// <param name="id">The string's id</param>
		public DialogEntry(string filePath, string type, string id) {
			StringType = type;
			StringId = id;

			JsonNode jsonData = GetEntryFromJson(filePath);

			IconPath = jsonData!["icon"]!.GetValue<string>();

			JsonArray pagesNode = jsonData!["pages"]!.AsArray();
			for (int i = 0; i < pagesNode.Count; i++) {
				string pagetext = pagesNode![i]!.GetValue<string>();
				Pages.Add(pagetext);
				PagesDone.Add(false);
			}
		}

	
		/// <summary>
		/// Retrieves a JsonNode from the specified file based on the stringType and stringId parameters.
		/// </summary>
		/// <param name="file">The file to search</param>
		/// <returns>Either the specified JsonNode or null</returns>
		public JsonNode GetEntryFromJson(string file) {
			string fileContents = FileAccess.Open(file, FileAccess.ModeFlags.Read).GetAsText();
			JsonNode fileAsJson = JsonNode.Parse(fileContents);
			JsonNode stringArray = fileAsJson![StringType];
			JsonNode stringEntry = null;

			for (int i = 0; i < stringArray.AsArray().Count; i++) {
				if (stringArray[i]!["id"]!.GetValue<string>() == StringId) {
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
}
