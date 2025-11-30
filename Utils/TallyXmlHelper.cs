using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

public static class TallyXmlHelper
{
	public static string CleanTallyXml(string xmlContent)
	{
		if (string.IsNullOrEmpty(xmlContent))
			return xmlContent;

		// Remove specific problematic entities and characters
		xmlContent = xmlContent.Replace("&#4;", "")
							  .Replace("&#x4;", "")
							  .Replace("&#04;", "")
							  .Replace("\u0004", "")
							  .Replace("\x04", "");

		// Remove other control characters
		xmlContent = Regex.Replace(xmlContent, @"[\x00-\x08\x0B\x0C\x0E-\x1F]", "");

		return xmlContent;
	}

	public static Envelope DeserializeTallyXml(string xmlContent)
	{
		try
		{
			xmlContent = CleanTallyXml(xmlContent);

			var serializer = new XmlSerializer(typeof(Envelope));
			var settings = new XmlReaderSettings
			{
				CheckCharacters = false,
				IgnoreProcessingInstructions = true,
				DtdProcessing = DtdProcessing.Ignore,
				XmlResolver = null
			};

			using (var stringReader = new StringReader(xmlContent))
			using (var xmlReader = XmlReader.Create(stringReader, settings))
			{
				return (Envelope)serializer.Deserialize(xmlReader);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Deserialization error: {ex.Message}");
			return null;
		}
	}

	public static void SaveXmlForDebugging(string xmlContent, string filename)
	{
		try
		{
			File.WriteAllText($"original_{filename}", xmlContent);
			File.WriteAllText($"cleaned_{filename}", CleanTallyXml(xmlContent));
		}
		catch
		{
			// Ignore file errors
		}
	}
}