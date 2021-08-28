using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

[System.Serializable]
public class Data
{
	public List<int> posX = new List<int>();
	public List<int> posY = new List<int>();
	public List<int> posZ = new List<int>();

	public List<float> value = new List<float>();
	public List<float> growSpeed = new List<float>();

	public List<string> cropName = new List<string>();
	public List<bool> grown = new List<bool>();


	public Data(Main main)
	{
		int plantedCropsLength = main.plantedCrops.Count;

		for(int i = 0; i < plantedCropsLength; i++)
		{
			posX.Add(main.plantedCrops[i].posX);
			posY.Add(main.plantedCrops[i].posY);
			posZ.Add(main.plantedCrops[i].posZ);

			value.Add(main.plantedCrops[i].value);
			growSpeed.Add(main.plantedCrops[i].growSpeed);

			cropName.Add(main.plantedCrops[i].cropName);
			grown.Add(main.plantedCrops[i].grown);
		}
		
	}
}