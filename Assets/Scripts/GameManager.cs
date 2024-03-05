using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public struct Ability
    {
        //0= "+"; 1="-"; 2="x"; 3=÷"
        public int abilityID;
        public int abilityPower;
        public Ability(int _abilityID, int _abilityPower)
        {
            abilityID = _abilityID;
            abilityPower = _abilityPower;
        }
    }

    //Gate abilities
    [HideInInspector]
    public string[] abilityName = { "+", "-", "x", "÷" };

    //Player Number Text
    [SerializeField]
    TextMeshPro tx_PlayerNum;
    //Prefabs
    [SerializeField]
    GateController gate_Prefab;
    [SerializeField]
    PlayerUnit playerUnit_Prefab;
    //Player container
    [SerializeField]
    Transform playerGroup;

    public Transform playerTarget;

    //Maximum actual number
    int _playerNum;
    int PlayerNum
	{
		get
		{
            return _playerNum;
		}
		set
		{
            _playerNum = Mathf.Clamp(value, 0, 500);
		}
	}
    //Maximum number for player minion prefab instances
    int playerVisualMaxNum = 100;
    //Player controlling speed
    float speed = 10;

    //Gate timer
    float gateTimer = 0;
    float gateTimerTotal = 1;


    public static GameManager instance;
    //Control all the playerUnits
    List<PlayerUnit> playerUnitList = new List<PlayerUnit>();
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
		{
            instance = this;
		}
		else
		{
            Destroy(gameObject);
		}

        PlayerNum = 0;
        CreateUnit(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerNum == 0) return;
        //Player crowd control
        var horizontal = Input.GetAxis("Horizontal");
        playerTarget.Translate(Vector3.right * horizontal * speed * Time.deltaTime);
        //Movement Limitation
        if(playerTarget.position.x < -5)
		{
            playerTarget.position = new Vector3(
                -5, 
                playerTarget.position.y, 
                playerTarget.position.z
                );
		}
        else if(playerTarget.position.x > 5)
		{
            playerTarget.position = new Vector3(
                5,
                playerTarget.position.y,
                playerTarget.position.z
                );
		}

        //Generate gate
        if(gateTimer > gateTimerTotal)
		{
            gateTimer = 0;
            //0.5f - 3.5f seconds
            gateTimerTotal = 0.5f + Random.value * 3;
            //Generate the gate and set position to the top of the screen
            GateController gate = Instantiate(gate_Prefab);
            gate.Position = new Vector3(0, 0, 25);
		}
		else
		{
            gateTimer += Time.deltaTime;
		}
    }

	private void LateUpdate()
	{
        //Refresh player num after Update()
        tx_PlayerNum.text = PlayerNum.ToString();
	}

	public void RemoveUnit(PlayerUnit unit)
	{
        playerUnitList.Remove(unit);
        Destroy(unit.gameObject);
	}

    void CreateUnit(int num)
	{
        //Change PlayerNum
        PlayerNum += num;

        var increasedNum = Mathf.Clamp(num, 0, playerVisualMaxNum - playerUnitList.Count);
        for(int i = 0; i < increasedNum; i++)
		{
            PlayerUnit unit = Instantiate(playerUnit_Prefab, playerGroup);
            unit.Position = playerTarget.position + 
                new Vector3(
                Random.value,
                Random.value,
                Random.value)
                ;
            playerUnitList.Add(unit);
		}
	}

    public void ApplyGateAbility(Ability ability)
	{
        if(ability.abilityID == 0)
		{
            //+
            CreateUnit(ability.abilityPower);
		}
        else if (ability.abilityID == 1)
        {
            //-
            print("- is triggered: " + (PlayerNum - ability.abilityPower));
            int decresedNum = CalculateDecreasedPlayerNum(PlayerNum - ability.abilityPower);
            
            if(playerUnitList.Count > 0)
			{
                for (int i = 0; i < decresedNum; i++)
				{
                    RemoveUnit(playerUnitList[Random.Range(0, playerUnitList.Count)]);
				}
			}
        }
        else if (ability.abilityID == 2)
		{
            //"x"
            CreateUnit(PlayerNum * (ability.abilityPower - 1));
        }
        else if(ability.abilityID == 3)
		{
            //÷
            int decresedNum = CalculateDecreasedPlayerNum(
                Mathf.RoundToInt((float)PlayerNum / ability.abilityPower)
                );

            if(playerUnitList.Count > 0)
			{
                for(int i = 0; i < decresedNum; i++)
				{
                    RemoveUnit(playerUnitList[Random.Range(0, playerUnitList.Count)]);
				}
			}
        }
    }

    int CalculateDecreasedPlayerNum (int formulaResult)
	{
        int decresedNum = 0;
        if(PlayerNum > playerVisualMaxNum)
		{
            //100 player gameobjects already exists
            decresedNum = playerVisualMaxNum - formulaResult;
		}
		else
		{
            
            decresedNum = Mathf.Min(PlayerNum, PlayerNum - formulaResult);
		}

        //Refresh playerNum
        PlayerNum = formulaResult;
        print(formulaResult);
        return decresedNum;
	}
}
