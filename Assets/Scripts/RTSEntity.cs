using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "New RTSEntity", menuName = "new RTSENtity")]
public class RTSEntity : ScriptableObject
{

    //todo idioma descricpion 
    [Header("Generic stuff")]
    string entityName;
    [SerializeField] private bool isMoralUp;
    [SerializeField] public EntityType entityType;
    [SerializeField] private int maxHealth;
    [SerializeField] private float expirationVelocity;
    [SerializeField] private float damageMoral;
    [SerializeField] private List<int> prices;
     //TODO borrar
    [SerializeField] private string description;
    [SerializeField] private Sprite preview;
    [SerializeField] private GameObject prefab;
    [Header("Movement stuff")]
    [SerializeField] private int moral;

    [SerializeField] private float defaultStoppingDistance=2;
    [SerializeField] private float velocity;
    [SerializeField] private Transform currentTarget;

    [Header("Area stuff")]
    [SerializeField] private float effectRadious;
    [SerializeField] private float recoverySpeed;
    [Header("Damage stuff")]
    [SerializeField] private float damage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackTimer;
    [Header("Proyectile stuff")]
    [SerializeField] private GameObject proyectile;
    [Header("Crafting stuff")]
    [SerializeField] private float craftRadious;
    [SerializeField] private GameObject craftCompletedGO;
    [SerializeField] private GameObject craftUnCompletedGO;
    [SerializeField] private Renderer buildRenderer;
    [SerializeField] private int buildTime;
    [SerializeField] private bool canCraft;
    [SerializeField] private List<GameObject> productionUnits;
    [SerializeField] private Queue<Unit> unitsQueue;
    [SerializeField] private Unit curretUnit;
    [Header("Description  stuff")]
    [SerializeField] private TextStrings descriptionText;
    [SerializeField] private int descriptionPosition;
    [Header("Villager stuff")]
    [SerializeField] private GameObject currentProduction;
    [Header("Name  stuff")]
    [SerializeField] private TextStrings nameText;
    [SerializeField] private int namePosition;
    [HideInInspector] [SerializeField] UnitStates unitState;
    [HideInInspector] [SerializeField] EmotionalStates emotionalState;
    [HideInInspector] [SerializeField] ResourcesType typeOfResourceThatCanHave;
    public float DefaultStoppingDistance => defaultStoppingDistance;

    public string EntityName { get => entityName; set => entityName = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float ExpirationVelocity { get => expirationVelocity; set => expirationVelocity = value; }
    public List<int> Prices { get => prices; set => prices = value; }
    public string Description { get => description; set => description = value; }
    public Sprite Preview { get => preview; set => preview = value; }
    public GameObject Prefab { get => prefab; set => prefab = value; }
    public float Velocity { get => velocity; set => velocity = value; }
    public Transform CurrentTarget { get => currentTarget; set => currentTarget = value; }
    public float EffectRadious { get => effectRadious; set => effectRadious = value; }
    public float RecoverySpeed { get => recoverySpeed; set => recoverySpeed = value; }
    public float Damage { get => damage; set => damage = value; }
    public float AttackRange { get => attackRange; set => attackRange = value; }
    public float AttackTimer { get => attackTimer; set => attackTimer = value; }
    public float CraftRadious { get => craftRadious; set => craftRadious = value; }
     public GameObject CraftCompletedGO { get => craftCompletedGO; set => craftCompletedGO = value; }
    public GameObject CraftUnCompletedGO { get => craftUnCompletedGO; set => craftUnCompletedGO = value; }
    public Renderer BuildRenderer { get => buildRenderer; set => buildRenderer = value; }
    public int BuildTime { get => buildTime; set => buildTime = value; }
    public bool CanCraft { get => canCraft; set => canCraft = value; }
    public List<GameObject> ProductionUnits { get => productionUnits; set => productionUnits = value; }
    public TextStrings DescriptionText { get => descriptionText; set => descriptionText = value; }
    public int DescriptionPosition { get => descriptionPosition; set => descriptionPosition = value; }
    public TextStrings NameText { get => nameText; set => nameText = value; }
    public int NamePosition { get => namePosition; set => namePosition = value; }
    public UnitStates UnitState { get => unitState; set => unitState = value; }
    public EmotionalStates EmotionalState { get => emotionalState; set => emotionalState = value; }
    public ResourcesType TypeOfResourceThatCanHave { get => typeOfResourceThatCanHave; set => typeOfResourceThatCanHave = value; }
    public GameObject Proyectile { get => proyectile; set => proyectile = value; }
    public Queue<Unit> UnitsQueue { get => unitsQueue; set => unitsQueue = value; }
    public int Moral { get => moral; set => moral = value; }
    public float DamageMoral { get => damageMoral; set => damageMoral = value; }
    public Unit CurretUnit { get => curretUnit; set => curretUnit = value; }
}
