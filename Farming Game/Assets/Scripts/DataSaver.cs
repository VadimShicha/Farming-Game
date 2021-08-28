using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public static class DataSaver
{
	static string path = Application.persistentDataPath + "/data.dat";

	public static void saveData(Main main)
	{
		BinaryFormatter formatter = new BinaryFormatter();

		System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Create);

		Data data = new Data(main);




		formatter.Serialize(stream, data);
		stream.Close();
	}

	public static Data loadData()
	{
		if (System.IO.File.Exists(path))
		{
			Data data;

			BinaryFormatter formatter = new BinaryFormatter();

			System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Open);

			data = formatter.Deserialize(stream) as Data;

			stream.Close();

			return data;
		}
		else
		{
			Debug.LogError("Can't open file");

			return null;
		}
	}

}

