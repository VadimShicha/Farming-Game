//Made on 7/31/2021
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

struct Slot
{
	public Item item;

	public int amount;

	public static void addItem(Slot[] slots, string itemName, int amount)
	{
		for(int i = 0; i < slots.Length; i++)
		{
			if(slots[i].item.itemName == itemName)
			{
				slots[i].amount += amount;
			}
		}
	}

	public static void removeItem(Slot[] slots, string itemName, int amount)
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].item.itemName == itemName)
			{
				slots[i].amount -= amount;
			}
		}
	}
}

public struct PlantedCrop
{
	public int posX;
	public int posY;
	public int posZ;

	public float value; //between 0 and 1
	public float growSpeed;

	public string cropName;
	public bool grown;
}

struct SellShopSlot
{
	public int sellPrice;

	public float value;
	public float sellSpeed;

	public bool sold;
	public bool selling;
}

struct Item
{
	public string itemName;

	public bool canSell;
	public bool isCrop;

	public int averageSellPrice;

	public int minSellAmount;
	public int maxSellAmount;

	public int minSellPrice; //per one item
	public int maxSellPrice; //per one item

	public float cropGrowSpeed;
	public float sellSpeed;
}

struct Items
{
	/// <summary>
	/// The most you can an item for
	/// </summary>
	public const int maxPrice = 500;

	/// <summary>
	/// Get an item by it's name
	/// </summary>
	public static Item getItem(string itemName)
	{
		if(itemName == "Hoe")
		{
			Item item = new Item();

			item.itemName = "Hoe";

			item.sellSpeed = 0;

			item.minSellAmount = 0;
			item.maxSellAmount = 0;

			item.minSellPrice = 0;
			item.maxSellPrice = 0;

			item.isCrop = false;
			item.canSell = false;

			return item;
		}
		else if(itemName == "Wheat")
		{
			Item item = new Item();

			item.itemName = "Wheat";

			item.averageSellPrice = 2;
			item.sellSpeed = 0.01f;

			item.minSellAmount = 1;
			item.maxSellAmount = 50;

			item.minSellPrice = 1;
			item.maxSellPrice = 4;

			item.canSell = true;
			item.isCrop = true;

			item.cropGrowSpeed = 0.01f;

			return item;
		}
		else if(itemName == "Corn")
		{
			Item item = new Item();

			item.itemName = "Corn";

			item.averageSellPrice = 3;
			item.sellSpeed = 0.0015f;

			item.minSellAmount = 1;
			item.maxSellAmount = 40;

			item.minSellPrice = 1;
			item.maxSellPrice = 6;

			item.canSell = true;
			item.isCrop = true;

			item.cropGrowSpeed = 0.005f;

			return item;
		}
		else if(itemName == "Carrot")
		{
			Item item = new Item();

			item.itemName = "Carrot";

			item.averageSellPrice = 5;
			item.sellSpeed = 0.002f;

			item.minSellAmount = 1;
			item.maxSellAmount = 35;

			item.minSellPrice = 1;
			item.maxSellPrice = 12;

			item.canSell = true;
			item.isCrop = true;

			item.cropGrowSpeed = 0.0008f;

			return item;
		}
		else
		{
			//Debug.LogWarning("Can't find item by given name " + itemName);

			Item item = new Item();
			item.itemName = null;

			return item;
		}
	}
}






public class Main : MonoBehaviour
{
	public GameObject player;
	public GameObject sellShop;

	public Grid grid;
	public Tilemap grassTilemap;
	public Tilemap cropsTilemap;

	public Tile grassTile;
	public Tile dirtTile;

	public Tile wheatPlantTile;
	public Tile wheatPlantGrownTile;
	public Tile cornPlantTile;
	public Tile cornPlantGrownTile;
	public Tile carrotPlantTile;
	public Tile carrotPlantGrownTile;

	public TMP_Text coinsCounter;
	public TMP_Text enterShopText;

	public GameObject[] shopSlots;
	public GameObject[] barrels;

	public float moveSpeed = 4;

	public Color currentSlotColor = new Color(0, 0, 0, 1);

	Slot[] inventory = new Slot[4];
	SellShopSlot[] sellShopSlots;
	KeyCode[] alphaKeys =
	{
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3,
		KeyCode.Alpha4
	};
	Color defaultSlotColor;

	public List<PlantedCrop> plantedCrops = new List<PlantedCrop>();

	int coins = 0;
	int inSlot = 0;

	bool uiOpen = false;
	bool shopOpen = false;
	bool canEnterShop = false;

	bool[] sellShopSliderValuesChanged;
	bool[] sellShopInputValuesChanged;

	public int testInt = 0;

	void Start()
	{
		clearInventory();
		sellShopSliderValuesChanged = new bool[shopSlots.Length];
		sellShopInputValuesChanged = new bool[shopSlots.Length];
		sellShopSlots = new SellShopSlot[shopSlots.Length];

		clearInventory();

		defaultSlotColor = barrels[0].GetComponent<Image>().color;
	}



	void FixedUpdate()
	{
		float xAxisInput = Input.GetAxisRaw("Horizontal");
		float yAxisInput = Input.GetAxisRaw("Vertical");

		if(uiOpen == false)
		{
			if(xAxisInput != 0 || yAxisInput != 0)
			{
				Vector3 playerVel = player.GetComponent<Rigidbody2D>().velocity;

				player.GetComponent<Rigidbody2D>().velocity = new Vector3(moveSpeed * xAxisInput, playerVel.y, playerVel.z);
				playerVel = player.GetComponent<Rigidbody2D>().velocity;
				player.GetComponent<Rigidbody2D>().velocity = new Vector3(playerVel.x, moveSpeed * yAxisInput, playerVel.z);
			}
			else
			{
				Vector3 playerVel = player.GetComponent<Rigidbody2D>().velocity;

				player.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, playerVel.z);
			}
		}
		


		int plantedCropsLength = plantedCrops.Count;

		for(int i = 0; i < plantedCropsLength; i++)
		{
			PlantedCrop plantedCrop = plantedCrops[i];

			if(plantedCrops[i].value >= 1 && plantedCrops[i].grown == false)
			{
				Vector3Int plantedCropPos = new Vector3Int(plantedCrops[i].posX, plantedCrops[i].posY, plantedCrops[i].posZ);

				plantedCrop.grown = true;

				if(plantedCrop.cropName == "Wheat")
				{
					cropsTilemap.SetTile(plantedCropPos, wheatPlantGrownTile);
				}
				else if(plantedCrop.cropName == "Corn")
				{
					cropsTilemap.SetTile(plantedCropPos, cornPlantGrownTile);
				}
				else if(plantedCrop.cropName == "Carrot")
				{
					cropsTilemap.SetTile(plantedCropPos, carrotPlantGrownTile);
				}
			}
			else
			{
				plantedCrop.value += plantedCrop.growSpeed;
			}

			plantedCrops[plantedCrops.FindIndex(ind => ind.Equals(plantedCrops[i]))] = plantedCrop;
		}
	}

	void Update()
	{
		//camera follow
		Vector3 playerPos = player.transform.position;

		Camera.main.transform.position = new Vector3(playerPos.x, playerPos.y, Camera.main.transform.position.z);


		if(Input.GetKeyDown(KeyCode.Equals))
		{
			DataSaver.saveData(this);
		}
		if(Input.GetKeyDown(KeyCode.Minus))
		{
			Data data = DataSaver.loadData();

			int cropNameLength = data.cropName.Count;

			for(int i = 0; i < cropNameLength; i++)
			{
				print("SD");
				PlantedCrop plantedCrop;
				Vector3Int cropPos = new Vector3Int(data.posX[i], data.posY[i], data.posZ[i]);
				string cropName = data.cropName[i];

				plantedCrop.posX = data.posX[i];
				plantedCrop.posY = data.posY[i];
				plantedCrop.posZ = data.posZ[i];

				plantedCrop.value = data.value[i];
				plantedCrop.growSpeed = data.growSpeed[i];

				plantedCrop.cropName = data.cropName[i];
				plantedCrop.grown = data.grown[i];


				if(data.grown[i] == true)
				{
					if (cropName == "Wheat")
					{
						cropsTilemap.SetTile(cropPos, wheatPlantGrownTile);
					}
					else if (cropName == "Corn")
					{
						cropsTilemap.SetTile(cropPos, cornPlantGrownTile);
					}
					else if (cropName == "Carrot")
					{
						cropsTilemap.SetTile(cropPos, carrotPlantGrownTile);
					}
				}
				else if(data.grown[i] == false)
				{
					if(cropName == "Wheat")
					{
						cropsTilemap.SetTile(cropPos, wheatPlantTile);
					}
					else if(cropName == "Corn")
					{
						cropsTilemap.SetTile(cropPos, cornPlantTile);
					}
					else if(cropName == "Carrot")
					{
						cropsTilemap.SetTile(cropPos, carrotPlantTile);
					}
				}

				plantedCrops.Add(plantedCrop);
			}
		}

		if(canEnterShop == true && shopOpen == false)
		{
			enterShopText.gameObject.SetActive(true);

			if(Input.GetKeyDown(KeyCode.E))
			{
				enterShop();
			}
		}

		if(shopOpen == false)
		{
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}
		}

		if(Input.GetKeyDown(KeyCode.Space))
		{
			displayAllCrops();
		}

		
		if(shopOpen == true)
		{
			enterShopText.gameObject.SetActive(false);

			player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

			if(Input.GetKeyDown(KeyCode.Escape))
			{
				exitShop();
			}

			for(int i = 0; i < shopSlots.Length; i++)
			{
				string itemName = shopSlots[i].transform.Find("Slot" + (i + 1) + "ItemNameInput").GetComponent<TMP_InputField>().text;

				Slider priceSlider = shopSlots[i].transform.Find("Slot" + (i + 1) + "PriceSlider").GetComponent<Slider>();
				Slider amountSlider = shopSlots[i].transform.Find("Slot" + (i + 1) + "AmountSlider").GetComponent<Slider>();

				shopSlots[i].transform.Find("Slot" + (i + 1) + "PriceCounter").GetComponent<TMP_Text>().text = priceSlider.value.ToString();
				shopSlots[i].transform.Find("Slot" + (i + 1) + "AmountCounter").GetComponent<TMP_Text>().text = amountSlider.value.ToString();

				
				
				string itemNameUpper = itemName;

				//to upper first char
				if(itemName.Length > 0)
				{
					System.Text.StringBuilder itemNameBuilder = new System.Text.StringBuilder(itemName);
				
					itemNameBuilder[0] = char.ToUpper(itemName[0]);
					itemNameUpper = itemNameBuilder.ToString();
				}
					
				
				//if input field value changed
				if(sellShopInputValuesChanged[i] == true)
				{
					if(Items.getItem(itemNameUpper).itemName != null && Items.getItem(itemNameUpper).canSell == true)
					{
						amountSlider.minValue = Items.getItem(itemNameUpper).minSellAmount;

						if(Items.getItem(itemNameUpper).maxSellAmount <= getAmountByItemNameInventory(itemNameUpper))
						{
							amountSlider.maxValue = Items.getItem(itemNameUpper).maxSellAmount;
						}
						else
						{
							amountSlider.maxValue = getAmountByItemNameInventory(itemNameUpper);
						}


						
						priceSlider.minValue = Items.getItem(itemNameUpper).minSellPrice;
						priceSlider.maxValue = Items.getItem(itemNameUpper).maxSellPrice;

						priceSlider.value = Mathf.RoundToInt(Items.getItem(itemNameUpper).maxSellPrice / 2);

						if(getAmountByItemNameInventory(itemNameUpper) <= 0)
						{
							shopSlots[i].transform.Find("Slot" + (i + 1) + "Button").GetComponent<Button>().interactable = false;
						}
						else
						{
							shopSlots[i].transform.Find("Slot" + (i + 1) + "Button").GetComponent<Button>().interactable = true;
						}

							
						if(getAmountByItemNameInventory(itemNameUpper) <= 0)
						{
							amountSlider.interactable = false;
							priceSlider.interactable = false;
						}
						else
						{
							amountSlider.interactable = true;
							priceSlider.interactable = true;
						}


						sellShopInputValuesChanged[i] = false;
					}
					else
					{
						shopSlots[i].transform.Find("Slot" + (i + 1) + "Button").GetComponent<Button>().interactable = false;

						amountSlider.interactable = false;
						priceSlider.interactable = false;
					}
				}

				//if slider value changed
				if(sellShopSliderValuesChanged[i] == true)
				{
					priceSlider.maxValue = Items.getItem(itemNameUpper).maxSellPrice * amountSlider.value;

					sellShopSliderValuesChanged[i] = false;
				}
			}

			
		}

		if(uiOpen == false)
		{
			if(Input.GetKey(KeyCode.Mouse0))
			{
				Vector3 mousePos = Input.mousePosition;
				mousePos.z = Camera.main.nearClipPlane;

				Vector3Int tilePos = grid.WorldToCell(Camera.main.ScreenToWorldPoint(mousePos));

				if(cropsTilemap.GetTile(tilePos) == wheatPlantGrownTile || cropsTilemap.GetTile(tilePos) == cornPlantGrownTile || cropsTilemap.GetTile(tilePos) == carrotPlantGrownTile)
				{
					try
					{
						int plantedCropsLength = plantedCrops.Count;

						for(int i = 0; i < plantedCropsLength; i++)
						{
							Vector3Int plantedCropPos = new Vector3Int(plantedCrops[i].posX, plantedCrops[i].posY, plantedCrops[i].posZ);

							if(plantedCropPos == tilePos)
							{
								addItemToInventory(plantedCrops[i].cropName, 2);

								plantedCrops.RemoveAt(i);
							}
						}

						cropsTilemap.SetTile(tilePos, null);
					}
					catch(System.ArgumentOutOfRangeException)
					{
						//handle exception
					}
				}
			}
		
			if(Input.GetKey(KeyCode.Mouse1))
			{
				string itemInHand = inventory[inSlot].item.itemName;

				if(inventory[inSlot].item.itemName == "Hoe")
				{
					Vector3 mousePos = Input.mousePosition;
					mousePos.z = Camera.main.nearClipPlane;

					Vector3Int tilePos = grid.WorldToCell(Camera.main.ScreenToWorldPoint(mousePos));

					if(grassTilemap.GetTile(tilePos) == grassTile)
					{
						if(inventory[inSlot].amount > 0)
						{
							grassTilemap.SetTile(tilePos, dirtTile);

							inventory[inSlot].amount--;
						}
					}
				}
				else if(itemInHand == "Wheat" || itemInHand == "Corn" || itemInHand == "Carrot")
				{
					Vector3 mousePos = Input.mousePosition;
					mousePos.z = Camera.main.nearClipPlane;

					Vector3Int tilePos = grid.WorldToCell(Camera.main.ScreenToWorldPoint(mousePos));

					if(grassTilemap.GetTile(tilePos) == dirtTile && cropsTilemap.GetTile(tilePos) == null)
					{
						if(inventory[inSlot].amount > 0)
						{
							if(itemInHand == "Wheat")
							{
								cropsTilemap.SetTile(tilePos, wheatPlantTile);

								PlantedCrop plantedCrop = new PlantedCrop();

								plantedCrop.cropName = "Wheat";
								plantedCrop.growSpeed = Items.getItem("Wheat").cropGrowSpeed;
								print(tilePos);
								plantedCrop.posX = tilePos.x;
								plantedCrop.posY = tilePos.y;
								plantedCrop.posZ = tilePos.z;
								plantedCrop.grown = false;

								plantedCrops.Add(plantedCrop);
							}
							else if(itemInHand == "Corn")
							{
								cropsTilemap.SetTile(tilePos, cornPlantTile);

								PlantedCrop plantedCrop = new PlantedCrop();

								plantedCrop.cropName = "Corn";
								plantedCrop.growSpeed = Items.getItem("Corn").cropGrowSpeed;
								plantedCrop.posX = tilePos.x;
								plantedCrop.posY = tilePos.y;
								plantedCrop.posZ = tilePos.z;
								plantedCrop.grown = false;

								plantedCrops.Add(plantedCrop);
							}
							else if(itemInHand == "Carrot")
							{
								cropsTilemap.SetTile(tilePos, carrotPlantTile);

								PlantedCrop plantedCrop = new PlantedCrop();

								plantedCrop.cropName = "Carrot";
								plantedCrop.growSpeed = Items.getItem("Carrot").cropGrowSpeed;
								plantedCrop.posX = tilePos.x;
								plantedCrop.posY = tilePos.y;
								plantedCrop.posZ = tilePos.z;
								plantedCrop.grown = false;

								plantedCrops.Add(plantedCrop);
							}


							inventory[inSlot].amount--;
						}
					}
				}
			}

			for(int i = 0; i < alphaKeys.Length; i++)
			{
				if(Input.GetKeyDown(alphaKeys[i]))
				{
					barrels[inSlot].GetComponent<Image>().color = defaultSlotColor;
					inSlot = i;
					barrels[i].GetComponent<Image>().color = currentSlotColor;
				}
			}
		}


		//makes the shop slots sell
		for(int i = 0; i < sellShopSlots.Length; i++)
		{
			string itemName = shopSlots[i].transform.Find("Slot" + (i + 1) + "ItemNameInput").GetComponent<TMP_InputField>().text;
			string itemNameUpper = itemName;

			if(itemName.Length > 0)
			{
				System.Text.StringBuilder itemNameBuilder = new System.Text.StringBuilder(itemName);

				itemNameBuilder[0] = char.ToUpper(itemName[0]);
				itemNameUpper = itemNameBuilder.ToString();
			}

			Slider priceSlider = shopSlots[i].transform.Find("Slot" + (i + 1) + "PriceSlider").GetComponent<Slider>();
			Slider amountSlider = shopSlots[i].transform.Find("Slot" + (i + 1) + "AmountSlider").GetComponent<Slider>();

			int amount = Mathf.RoundToInt(amountSlider.value);
			int price = Mathf.RoundToInt(amountSlider.value);
			float sellSpeed = sellShopSlots[i].sellSpeed;

			if(sellShopSlots[i].selling == true && sellShopSlots[i].sold == false)
			{
				//sellShopSlots[i].value += (sellShopSlots[i].sellSpeed / amountSlider.value) / (priceSlider.value / 2);

				//(0.0001 / 5) + (0.0001 / 50)
				//(sellSpeed / price) + (sellSpeed / amount)

				sellShopSlots[i].value += (sellSpeed / price) + (sellSpeed / amount);
				print((sellSpeed / price) + (sellSpeed / amount));

				
				if(sellShopSlots[i].value >= 1)
				{
					sellShopSlots[i].sold = true;

					shopSlots[i].transform.Find("Slot" + (i + 1) + "TitleText").GetComponent<TMP_Text>().text = "Sold";

					shopSlots[i].transform.Find("Slot" + (i + 1) + "Button").GetComponent<Button>().interactable = true;
				}
			}
		}




		coinsCounter.text = coins.ToString();

		updateInventoryCounters();
	}

	/*
	 
	GameObject gameObjectToFade;
	float flashSpeed = 0.1f;
	bool flashing = false;

	IEnumerator Flash()
	{
		

		flashing = true;

		for(float i = 0; i <= 4; i++)
		{
			if(i % 2 == 0)
			{
				//even
				gameObjectToFade.SetActive(false);
			}
			else
			{
				gameObjectToFade.SetActive(true);
			}

			yield return new WaitForSeconds(flashSpeed);
		}

		flashing = false;
		StopCoroutine("Flash");
	}
	*/


	void OnTriggerStay2D(Collider2D collision)
	{
		if(collision.gameObject.CompareTag("SellShop"))
		{
			canEnterShop = true;
		}
	}

	void OnTriggerExit2D(Collider2D collision)
	{
		if(collision.gameObject.CompareTag("SellShop"))
		{
			enterShopText.gameObject.SetActive(false);
			canEnterShop = false;
		}
	}

	public void onSellShopSliderValuesChanged(int slot)
	{
		sellShopSliderValuesChanged[slot] = true;
	}

	public void onSellShopInputValuesChanged(int slot)
	{
		sellShopInputValuesChanged[slot] = true;
	}

	public void onSellShopSlotButtonClicked(int slot)
	{
		Button button = shopSlots[slot].transform.Find("Slot" + (slot + 1) + "Button").GetComponent<Button>();

		string itemName = shopSlots[slot].transform.Find("Slot" + (slot + 1) + "ItemNameInput").GetComponent<TMP_InputField>().text;
		string title = shopSlots[slot].transform.Find("Slot" + (slot + 1) + "TitleText").GetComponent<TMP_Text>().text;
		string itemNameUpper = itemName;

		int amount = Mathf.RoundToInt(shopSlots[slot].transform.Find("Slot" + (slot + 1) + "AmountSlider").GetComponent<Slider>().value);
		int price = Mathf.RoundToInt(shopSlots[slot].transform.Find("Slot" + (slot + 1) + "PriceSlider").GetComponent<Slider>().value);

		if(itemName.Length > 0)
		{
			System.Text.StringBuilder itemNameBuilder = new System.Text.StringBuilder(itemName);

			itemNameBuilder[0] = char.ToUpper(itemName[0]);
			itemNameUpper = itemNameBuilder.ToString();
		}


		if(title == "Create Sale")
		{
			shopSlots[slot].transform.Find("Slot" + (slot + 1) + "TitleText").GetComponent<TMP_Text>().text = "Selling";

			shopSlots[slot].transform.Find("Slot" + (slot + 1) + "ItemNameInput").GetComponent<TMP_InputField>().interactable = false;

			shopSlots[slot].transform.Find("Slot" + (slot + 1) + "AmountSlider").GetComponent<Slider>().interactable = false;
			shopSlots[slot].transform.Find("Slot" + (slot + 1) + "PriceSlider").GetComponent<Slider>().interactable = false;

			button.transform.Find(button.name + "Text").GetComponent<TMP_Text>().text = "Claim";
			button.interactable = false;

			sellShopSlots[slot].sellPrice = price;
			sellShopSlots[slot].value = 0;
			sellShopSlots[slot].sellSpeed = Items.getItem(itemNameUpper).sellSpeed;
			sellShopSlots[slot].selling = true;

			Slot.removeItem(inventory, itemNameUpper, Mathf.RoundToInt(shopSlots[slot].transform.Find("Slot" + (slot + 1) + "AmountSlider").GetComponent<Slider>().value));
		}
		else if(title == "Sold")
		{
			button.interactable = true;

			button.transform.Find(button.name + "Text").GetComponent<TMP_Text>().text = "Create";

			shopSlots[slot].transform.Find("Slot" + (slot + 1) + "ItemNameInput").GetComponent<TMP_InputField>().text = null;
			shopSlots[slot].transform.Find("Slot" + (slot + 1) + "TitleText").GetComponent<TMP_Text>().text = "Create Sale";

			shopSlots[slot].transform.Find("Slot" + (slot + 1) + "AmountSlider").GetComponent<Slider>().minValue = 1;
			shopSlots[slot].transform.Find("Slot" + (slot + 1) + "AmountSlider").GetComponent<Slider>().maxValue = 50;

			shopSlots[slot].transform.Find("Slot" + (slot + 1) + "PriceSlider").GetComponent<Slider>().minValue = 1;
			shopSlots[slot].transform.Find("Slot" + (slot + 1) + "PriceSlider").GetComponent<Slider>().minValue = 50;

			shopSlots[slot].transform.Find("Slot" + (slot + 1) + "PriceSlider").GetComponent<Slider>().value = 1;
			shopSlots[slot].transform.Find("Slot" + (slot + 1) + "AmountSlider").GetComponent<Slider>().value = 1;

			shopSlots[slot].transform.Find("Slot" + (slot + 1) + "ItemNameInput").GetComponent<TMP_InputField>().interactable = true;
			shopSlots[slot].transform.Find("Slot" + (slot + 1) + "AmountSlider").GetComponent<Slider>().interactable = true;
			shopSlots[slot].transform.Find("Slot" + (slot + 1) + "PriceSlider").GetComponent<Slider>().interactable = true;

			coins += sellShopSlots[slot].sellPrice;

			sellShopSlots[slot].sold = false;
			sellShopSlots[slot].sellPrice = 0;
			sellShopSlots[slot].value = 0;
			sellShopSlots[slot].sellSpeed = 0.001f;
			sellShopSlots[slot].selling = false;
		}
	}

	void enterShop()
	{
		int canvasChildCount = GameObject.Find("Canvas").transform.childCount;

		for(int i = 0; i < canvasChildCount; i++)
		{
			GameObject.Find("Canvas").transform.GetChild(i).gameObject.SetActive(false);
		}

		GameObject.Find("Canvas").transform.Find("ShopUI").gameObject.SetActive(true);
		coinsCounter.transform.parent.gameObject.SetActive(true);

		uiOpen = true;
		shopOpen = true;
	}

	void exitShop()
	{
		int canvasChildCount = GameObject.Find("Canvas").transform.childCount;

		for(int i = 0; i < canvasChildCount; i++)
		{
			GameObject.Find("Canvas").transform.GetChild(i).gameObject.SetActive(true);
		}

		GameObject.Find("Canvas").transform.Find("ShopUI").gameObject.SetActive(false);

		uiOpen = false;
		shopOpen = false;
	}

	void clearInventory()
	{
		inventory[0].item = Items.getItem("Hoe");
		inventory[1].item = Items.getItem("Wheat");
		inventory[2].item = Items.getItem("Corn");
		inventory[3].item = Items.getItem("Carrot");

		for(int i = 0; i < inventory.Length; i++)
		{
			inventory[i].amount = 100;
		}
	}

	void updateInventoryCounters()
	{
		for(int i = 0; i < barrels.Length; i++)
		{
			try
			{
				barrels[i].transform.Find("Barrel" + (i + 1) + "Counter").GetComponent<TMP_Text>().text = inventory[i].amount.ToString();
			}
			catch(System.NullReferenceException)
			{
				//handle exception
			}
		}
	}

	void addItemToInventory(string itemName, int amount)
	{
		for(int i = 0; i < inventory.Length; i++)
		{
			if(inventory[i].item.itemName == itemName)
			{
				inventory[i].amount += amount;
			}
		}
	}	

	int getAmountByItemNameInventory(string itemName)
	{
		for(int i = 0; i < inventory.Length; i++)
		{
			if(inventory[i].item.itemName == itemName)
			{
				return inventory[i].amount;
			}
		}

		return 0;
	}

	void displayAllCrops()
	{
		int plantedCropsLength = plantedCrops.Count;

		for(int i = 0; i < plantedCropsLength; i++)
		{
			print("Pos: " + "Not Working" + " Crop Name: " + plantedCrops[i].cropName + " Value: " + plantedCrops[i].value + " Grown: " + plantedCrops[i].grown);
		}
	}
}
