/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject swordguyPrefab;
    public GameObject edgelordPrefab;
    public GameObject supportgirlPrefab;
    public GameObject catdogPrefab;
    public GameObject enemyPrefab;


    public Transform playerBattleStation;
    public Transform enemyBattlestation;

    Unit _playerUnit;
    Unit _edgelordUnit;
    Unit _supportgirlUnit;
    Unit _catdogUnit;
    Unit _enemyUnit;

    //public SoundManager soundManager;
    public AnimationManager animationManager;


    GameObject[] _button;

    public Text combatText;

    public BattleHUD playerHUD;
    public BattleHUD edgelordHUD;
    public BattleHUD supportgirlHUD;
    public BattleHUD catdogHUD;
    public BattleHUD enemyHUD;

    public BattleState state;

    private GameObject _supportGirl;


    public bool swordgBlocking = false;
    public bool sgblocking = false;
    public bool elblocking = false;
    public bool cdblocking = false;

    public bool swordguyturn = true;
    public bool supportgirlturn = true;
    public bool edgelordturn = true;
    public bool catdogturn = true;


    public bool supportgirlActive = false;

    public bool isDead = false;

    //public int rollRandom;

    void Start()
    {

        state = BattleState.START;
        StartCoroutine(SetupBattle());

    }

    private void Update()
    {
        
        if (isDead == true)
            StartCoroutine(EndBattle());

        if (_supportgirlUnit.Dead() == true)
        {
            playerBattleStation.GetChild(2).gameObject.SetActive(false);
            
        }
        if (_edgelordUnit.Dead() == true)
        {
            
            playerBattleStation.GetChild(1).gameObject.SetActive(false);
            
        }
        if (_playerUnit.Dead() == true)
        {
            playerBattleStation.GetChild(0).gameObject.SetActive(false);
            swordguyturn = false;
            
        }
        if (_catdogUnit.Dead() == true)
        {
            playerBattleStation.GetChild(3).gameObject.SetActive(false);

        }
    }

    IEnumerator SetupBattle()
    {
        
        
        GameObject swordguy = Instantiate(swordguyPrefab, playerBattleStation);
        swordguy.GetComponent<Unit>();
        _playerUnit = swordguy.GetComponent<Unit>();

        GameObject edgelord = Instantiate(edgelordPrefab, playerBattleStation);
        swordguy.GetComponent<Unit>();
        _edgelordUnit = edgelord.GetComponent<Unit>();

        _supportGirl = Instantiate(supportgirlPrefab, playerBattleStation);
        _supportGirl.GetComponent<Unit>();
        _supportgirlUnit = _supportGirl.GetComponent<Unit>();

        GameObject catDog = Instantiate(catdogPrefab, playerBattleStation);
        catDog.GetComponent<Unit>();
        _catdogUnit = catDog.GetComponent<Unit>();


        var enemyGO = Instantiate(enemyPrefab, enemyBattlestation);
        enemyGO.GetComponent<Unit>();
        _enemyUnit = enemyGO.GetComponent<Unit>();
        
        


        //add buttons
        _button = GameObject.FindGameObjectsWithTag("Button");

        foreach (var t in _button)
        {
            t.SetActive(false);
        }


        combatText.text = "A wild " + _enemyUnit.unitName + " appears!";

        playerHUD.SetHUD(_playerUnit);
        supportgirlHUD.SetHUD(_supportgirlUnit);
        edgelordHUD.SetHUD(_edgelordUnit);
        catdogHUD.SetHUD(_catdogUnit);
        enemyHUD.SetHUD(_enemyUnit);

        if (playerBattleStation.GetChild(2).gameObject.activeInHierarchy)
        {
            supportgirlActive = true;
        }
        

        switch (supportgirlActive)
        {
            case true:
                catdogHUD.disable();
                break;
            case false:
                supportgirlHUD.disable();
                break;
        }

        if (Encounter.LastSceneName == "SaveThePrincessForrest")
        {
            playerBattleStation.GetChild(1).gameObject.SetActive(false);
            GameObject.FindGameObjectWithTag("EdgelordHUD").SetActive(false);
        }
       

        yield return new WaitForSeconds(2);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    private void PlayerTurn()
    {
        animationManager.SwordguyIdle();
        catdogturn = true;
        supportgirlturn = true;
        edgelordturn = true;
        combatText.fontSize = 12;
        combatText.text = "Choose an action for Player";
        for (int i = 0; i < _button.Length; i++)
        {
            _button[i].SetActive(true);
        }
        //swordguyturn = false;
        Debug.Log("Swordguyturn");
    }

    void SupportgirlTurn()
    {
       
        
        animationManager.GoblinIdle();
       
        animationManager.RedknightIdle();
        
        animationManager.SupportgirlIdle();
        
        animationManager.EarthElementalIdle();
        
        animationManager.EvilsteveIdle();

        combatText.fontSize = 12;
        combatText.text = "Choose an action for Stacey";
        for (int i = 0; i < _button.Length; i++)
        {
            _button[i].SetActive(true);
        }
        supportgirlActive = true;
        supportgirlturn = false;
        Debug.Log("SupportgirlTurn");
    }

    private void CatdogTurn()
    {
        animationManager.GoblinIdle();
       
        animationManager.RedknightIdle();
        
        animationManager.CatdogIdle();
        
        animationManager.EarthElementalIdle();
        
        animationManager.EvilsteveIdle();
        
        combatText.fontSize = 12;
        combatText.text = "Choose an action for CatDog";
        for (int i = 0; i < _button.Length; i++)
        {
            _button[i].SetActive(true);
        }
        catdogturn = false;
        supportgirlActive = false;

        Debug.Log("CatDogturn");
    }
    void EdgelordTurn()
    {
        
        animationManager.EdgelordIdle();
        combatText.fontSize = 12;
        combatText.text = "Choose an action for Edgelord";
        for (int i = 0; i < _button.Length; i++)
        {
            _button[i].SetActive(true);
        }
        edgelordturn = false;
        Debug.Log("Edgelordturn");
    }


    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator PlayerAtttack()
    {

        if (isDead == true)
        {
            state = BattleState.WON;
            Debug.Log("starting coroutine endbattle?");
            StopAllCoroutines();

            StartCoroutine(EndBattle());

        }
        if (swordguyturn == false && edgelordturn == false && _edgelordUnit.Dead() == false)
        {
            
            
            
            animationManager.EdgelordAttack();
            SoundManager.Instance.PlayTeleport();
            

            yield return new WaitForSeconds(1);
            SoundManager.Instance.PlayAnimePunch();
            Debug.Log("Edgelord attacks");
            for (int i = 0; i < _button.Length; i++)
            {
                _button[i].SetActive(false);
            }
            combatText.fontSize = 25;

            isDead = _enemyUnit.TakeDamage(_edgelordUnit.damage);

            enemyHUD.setHP(_enemyUnit.currentHP);

            Vector2 v = new Vector2(-2, -14);

            combatText.rectTransform.anchoredPosition = v;

            combatText.text = "The attack is successful! \n " + _enemyUnit.unitName + " took " + _edgelordUnit.damage + " damage!";

            swordguyturn = false;

            supportgirlturn = false;

            yield return new WaitForSeconds(2);

            animationManager.EdgelordIdle();
            SoundManager.Instance.PlayTeleport();

            yield return new WaitForSeconds(2);
            Vector2 ve = new Vector2(-2, -22);
            combatText.rectTransform.anchoredPosition = ve;
            StartCoroutine(EnemyTurn());
        }
        if (swordguyturn == false && supportgirlturn == false && edgelordturn == true && supportgirlActive == true && _supportgirlUnit.Dead() == false)
        {
            for (int i = 0; i < _button.Length; i++)
            {
                _button[i].SetActive(false);
            }
            animationManager.SupportgirlAttack();
            
            SoundManager.Instance.PlayMagic();

            yield return new WaitForSeconds(1);
            
            animationManager.EarthElementalAttack();
            Debug.Log("Earth elemental attacks");

            yield return new WaitForSeconds(5);
            
            animationManager.EarthElementalIdle();
            
            animationManager.Explosion();
            
            SoundManager.Instance.PlayExplosion();

            yield return new WaitForSeconds(1);
            
            animationManager.ExplosionIdle();
            
            Debug.Log("Supportgirl attacks");
            
            

            isDead = _enemyUnit.TakeDamage(_supportgirlUnit.damage);

            enemyHUD.setHP(_enemyUnit.currentHP);

            Vector2 v = new Vector2(-2, -14);

            combatText.rectTransform.anchoredPosition = v;
            
            swordguyturn = false;

            edgelordturn = false;
            
            combatText.fontSize = 25;
            
            combatText.text = "The attack is succesful! \n " + _enemyUnit.unitName + " took " + _supportgirlUnit.damage + " damage!";

            yield return new WaitForSeconds(2);
            
            
            animationManager.SupportgirlIdle();
            
            

            Vector2 ve = new Vector2(-2, -22);
            combatText.rectTransform.anchoredPosition = ve;
            if (_edgelordUnit.Dead() == true || playerBattleStation.GetChild(1).gameObject.activeInHierarchy == false)
            {
                StartCoroutine(EnemyTurn());
            }
            else
                EdgelordTurn();
        }
        if (swordguyturn == false && supportgirlturn == true && edgelordturn == true && supportgirlActive == false && _catdogUnit.Dead() == false)
        {
            Debug.Log("CatDog attacks");

            for (int i = 0; i < _button.Length; i++)
            {
                _button[i].SetActive(false);
            }
            
            animationManager.CatdogAttack();
            
            SoundManager.Instance.PlayPew();
            
            animationManager.EvilsteveTakedamage();
            
            combatText.fontSize = 25;

            isDead = _enemyUnit.TakeDamage(_catdogUnit.damage);

            enemyHUD.setHP(_enemyUnit.currentHP);

            Vector2 v = new Vector2(-2, -14);

            combatText.rectTransform.anchoredPosition = v;

            combatText.text = "The attack is succesful! \n " + _enemyUnit.unitName + " took " + _catdogUnit.damage + " damage!";

            swordguyturn = false;

            yield return new WaitForSeconds(2);

            Vector2 ve = new Vector2(-2, -22);
            combatText.rectTransform.anchoredPosition = ve;

            if (playerBattleStation.GetChild(1).gameObject.activeInHierarchy == false)
            {
                StartCoroutine(EnemyTurn());
            }
            else
                EdgelordTurn();
        }


        if (swordguyturn == true && _playerUnit.Dead() == false && supportgirlturn == true && catdogturn == true)
        {
            animationManager.SwordguyAttack();
            
            Debug.Log("Swordguy attacks");
            
            for (int i = 0; i < _button.Length; i++)
            {
                _button[i].SetActive(false);
            }
            combatText.fontSize = 25;

            isDead = _enemyUnit.TakeDamage(_playerUnit.damage);

            enemyHUD.setHP(_enemyUnit.currentHP);

            Vector2 v = new Vector2(-2, -14);

            combatText.rectTransform.anchoredPosition = v;

            combatText.text = "The attack is succesful! \n " + _enemyUnit.unitName + " took " + _playerUnit.damage + " damage!";

            swordguyturn = false;
            
            SoundManager.Instance.PlayAttack();
            
            yield return new WaitForSeconds(2);
            

            Vector2 ve = new Vector2(-2, -22);
            combatText.rectTransform.anchoredPosition = ve;

            if (_catdogUnit.Dead() == true && _edgelordUnit.Dead() == true)
            {
                StartCoroutine(EnemyTurn());
            }

            if (_supportgirlUnit.Dead() == true && _edgelordUnit.Dead() == true)
            {
                StartCoroutine(EnemyTurn());
            }

            if (_supportgirlUnit.Dead() == true && supportgirlActive == true && _edgelordUnit.Dead() == false)
            {
                EdgelordTurn();
            }

            if (_catdogUnit.Dead() == true && supportgirlActive == false && _edgelordUnit.Dead() == false)
            {
                EdgelordTurn();
            }


            if (playerBattleStation.GetChild(3).gameObject.activeInHierarchy)
            {
                
                CatdogTurn();
            }
               

            if (playerBattleStation.GetChild(2).gameObject.activeInHierarchy)
            {
                SupportgirlTurn();
            }
                

        }

    }

    IEnumerator PlayerHeal()
    {
        for (int i = 0; i < _button.Length; i++)
        {
            _button[i].SetActive(false);
        }
        combatText.fontSize = 25;
        
        SoundManager.Instance.PlayHeal();


        if (swordguyturn == false && edgelordturn == false && _edgelordUnit.Dead() == false)
        {
            Debug.Log("Edgelord heals");
            _edgelordUnit.Heal(5000);

            edgelordHUD.setHP(_edgelordUnit.currentHP);
            combatText.text = _edgelordUnit.unitName + " heals to full!";

            yield return new WaitForSeconds(2);

            StartCoroutine(EnemyTurn());
        }
        if (swordguyturn == false && supportgirlturn == false && edgelordturn == true && supportgirlActive == true && _supportgirlUnit.Dead() == false)
        {
            Debug.Log("Supportgirl heals");
            _supportgirlUnit.Heal(5000);

            supportgirlHUD.setHP(_supportgirlUnit.currentHP);
            combatText.text = _supportgirlUnit.unitName + " heals to full!";

            yield return new WaitForSeconds(2);

            if (playerBattleStation.GetChild(1).gameObject.activeInHierarchy == false)
            {
                StartCoroutine(EnemyTurn());
            }
            else
                EdgelordTurn();
        }
        if (swordguyturn == false && supportgirlturn == true && edgelordturn == true && supportgirlActive == false && _catdogUnit.Dead() == false)
        {
            Debug.Log("Catdog heals");
            _catdogUnit.Heal(5000);

            catdogHUD.setHP(_catdogUnit.currentHP);
            combatText.text = _catdogUnit.unitName + " heals to full!";

            yield return new WaitForSeconds(2);

            if (playerBattleStation.GetChild(1).gameObject.activeInHierarchy == false)
            {
                StartCoroutine(EnemyTurn());
            }
            else
                EdgelordTurn();
        }
        

        if (swordguyturn == true && _playerUnit.Dead() == false && supportgirlturn == true && catdogturn == true)
        {
            Debug.Log("Swordguy heals");
            _playerUnit.Heal(5000);

            playerHUD.setHP(_playerUnit.currentHP);
            combatText.text = _playerUnit.unitName + " heals to full!";
            swordguyturn = false;

            yield return new WaitForSeconds(2);


            if (_catdogUnit.Dead() == true && _edgelordUnit.Dead() == true)
            {
                StartCoroutine(EnemyTurn());
            }

            if (_supportgirlUnit.Dead() == true && _edgelordUnit.Dead() == true)
            {
                StartCoroutine(EnemyTurn());
            }

            if (_supportgirlUnit.Dead() == true && supportgirlActive == true && _edgelordUnit.Dead() == false)
            {
                EdgelordTurn();
            }

            if (_catdogUnit.Dead() == true && supportgirlActive == false && _edgelordUnit.Dead() == false)
            {
                EdgelordTurn();
            }


            if (playerBattleStation.GetChild(3).gameObject.activeInHierarchy)
            {

                CatdogTurn();
            }


            if (playerBattleStation.GetChild(2).gameObject.activeInHierarchy)
            {
                SupportgirlTurn();
            }

        }

    }
    IEnumerator PlayerBlock()
    {
        for (int i = 0; i < _button.Length; i++)
        {
            _button[i].SetActive(false);
        }
        combatText.fontSize = 25;
        
        SoundManager.Instance.PlayShield();


        if (swordguyturn == false && edgelordturn == false && _edgelordUnit.Dead() == false)
        {
            elblocking = _edgelordUnit.Block();
            Vector2 v = new Vector2(-2, -15);

            combatText.rectTransform.anchoredPosition = v;
            combatText.text = _edgelordUnit.unitName + " blocks! Damage taken is reduced by 90% for one turn";

            swordguyturn = false;

            yield return new WaitForSeconds(2);


            Vector2 ve = new Vector2(-2, -22);
            combatText.rectTransform.anchoredPosition = ve;
            StartCoroutine(EnemyTurn());
        }
        if (swordguyturn == false && supportgirlturn == false && edgelordturn == true && supportgirlActive == true && _supportgirlUnit.Dead() == false)
        {
            sgblocking = _supportgirlUnit.Block();
            Vector2 v = new Vector2(-2, -15);

            combatText.rectTransform.anchoredPosition = v;
            combatText.text = _supportgirlUnit.unitName + " blocks! Damage taken is reduced by 90% for one turn";

            swordguyturn = false;

            yield return new WaitForSeconds(2);


            Vector2 ve = new Vector2(-2, -22);
            combatText.rectTransform.anchoredPosition = ve;
            if (playerBattleStation.GetChild(1).gameObject.activeInHierarchy == false)
            {
                StartCoroutine(EnemyTurn());
            }
            else
                EdgelordTurn();
        }
        if (swordguyturn == false && supportgirlturn == true && edgelordturn == true && supportgirlActive == false && _catdogUnit.Dead() == false)
        {
            cdblocking = _catdogUnit.Block();
            Vector2 v = new Vector2(-2, -15);

            combatText.rectTransform.anchoredPosition = v;
            combatText.text = _catdogUnit.unitName + " blocks! Damage taken is reduced by 90% for one turn";

            swordguyturn = false;

            yield return new WaitForSeconds(2);


            Vector2 ve = new Vector2(-2, -22);
            combatText.rectTransform.anchoredPosition = ve;
            if (playerBattleStation.GetChild(1).gameObject.activeInHierarchy == false)
            {
                StartCoroutine(EnemyTurn());
            }
            else
                EdgelordTurn();
        }
        
        if (swordguyturn == true && _playerUnit.Dead() == false && edgelordturn == true && supportgirlturn == true)
        {
            swordgBlocking = _playerUnit.Block();
            Vector2 v = new Vector2(-2, -15);

            combatText.rectTransform.anchoredPosition = v;
            combatText.text = _playerUnit.unitName + " blocks! Damage taken is reduced by 90% for one turn";

            swordguyturn = false;

            yield return new WaitForSeconds(2);


            Vector2 ve = new Vector2(-2, -22);
            combatText.rectTransform.anchoredPosition = ve;

            if (_catdogUnit.Dead() == true && _edgelordUnit.Dead() == true)
            {
                StartCoroutine(EnemyTurn());
            }

            if (_supportgirlUnit.Dead() == true && _edgelordUnit.Dead() == true)
            {
                StartCoroutine(EnemyTurn());
            }

            if (_supportgirlUnit.Dead() == true && supportgirlActive == true && _edgelordUnit.Dead() == false)
            {
                EdgelordTurn();
            }

            if (_catdogUnit.Dead() == true && supportgirlActive == false && _edgelordUnit.Dead() == false)
            {
                EdgelordTurn();
            }


            if (playerBattleStation.GetChild(3).gameObject.activeInHierarchy)
            {

                CatdogTurn();
            }


            if (playerBattleStation.GetChild(2).gameObject.activeInHierarchy)
            {
                SupportgirlTurn();
            }

        }
    }

    IEnumerator PlayerFlee()
    {
        for (int i = 0; i < _button.Length; i++)
        {
            _button[i].SetActive(false);
        }
        combatText.fontSize = 25;

        combatText.text = "You have escaped!";
        yield return new WaitForSeconds(3);
        
        SceneManager.LoadScene(Encounter.LastSceneName);

    }

    IEnumerator EnemyTurn()
    {
        state = BattleState.ENEMYTURN;

        combatText.text = _enemyUnit.unitName + " attacks!";

        var rollRandom = Random.Range(1, 4);
        
        Debug.Log(rollRandom);
        if (rollRandom == 1 && _playerUnit.Dead() == true)
        {
            StopAllCoroutines();
            StartCoroutine(EnemyTurn());
        }
        if (rollRandom == 2 && _supportgirlUnit.Dead() == true)
        {
            StopAllCoroutines();
            StartCoroutine(EnemyTurn());
        }
        if (rollRandom == 3 && _edgelordUnit.Dead() == true || rollRandom == 3 && playerBattleStation.GetChild(1).gameObject.activeInHierarchy == false)
        {
            StopAllCoroutines();
            StartCoroutine(EnemyTurn());
        }

        yield return new WaitForSeconds(2);

        if (rollRandom == 1 && _playerUnit.Dead() == false)
        {
            
                animationManager.GoblinAttack1();
            
            
           
                animationManager.RedknightAttack1();
            
                
            if (swordgBlocking == false)
            {

                _playerUnit.TakeDamage(_enemyUnit.damage);

                playerHUD.setHP(_playerUnit.currentHP);
                combatText.text = _playerUnit.unitName + " took " + _enemyUnit.damage + " damage!";

                Debug.Log("player took dmg");
            }
            else
            {
                _playerUnit.TakeDamage(_enemyUnit.damage * 1 / 10);

                combatText.text = _playerUnit.unitName + " took " + _enemyUnit.damage * 1 / 10 + " damage!";

                playerHUD.setHP(_playerUnit.currentHP);
                Debug.Log("player took smaller dmg");
            }
        }

        if (rollRandom == 2 && supportgirlActive == true && _supportgirlUnit.Dead() == false)
        {
            
                animationManager.GoblinAttack2();
            
            
           
                animationManager.RedknightAttack2();
            
            if (sgblocking == false)
            {

                _supportgirlUnit.TakeDamage(_enemyUnit.damage);

                supportgirlHUD.setHP(_supportgirlUnit.currentHP);
                combatText.text = _supportgirlUnit.unitName + " took " + _enemyUnit.damage + " damage!";

                Debug.Log("supportgirl took dmg");
            }
            else
            {
                _supportgirlUnit.TakeDamage(_enemyUnit.damage * 1 / 10);

                combatText.text = _supportgirlUnit.unitName + " took " + _enemyUnit.damage * 1 / 10 + " damage!";

                supportgirlHUD.setHP(_supportgirlUnit.currentHP);
                Debug.Log("supportgirl took smaller dmg");
            }

        }
        if (rollRandom == 2 && supportgirlActive == false && _catdogUnit.Dead() == false)
        {
            
                animationManager.GoblinAttack2();
                
                animationManager.RedknightAttack2();
            
            if (cdblocking == false)
            {

                _catdogUnit.TakeDamage(_enemyUnit.damage);

                catdogHUD.setHP(_catdogUnit.currentHP);
                combatText.text = _catdogUnit.unitName + " took " + _enemyUnit.damage + " damage!";

                Debug.Log("catdog took dmg");
            }
            else
            {
                _catdogUnit.TakeDamage(_enemyUnit.damage * 1 / 10);

                combatText.text = _catdogUnit.unitName + " took " + _enemyUnit.damage * 1 / 10 + " damage!";

                catdogHUD.setHP(_catdogUnit.currentHP);
                Debug.Log("catdog took smaller dmg");
            }
        }
        if (rollRandom == 3 && _edgelordUnit.Dead() == false)
        {
            
                animationManager.GoblinAttack3();
            
            
            
                animationManager.RedknightAttack3();
            
            if (elblocking == false)
            {

                _edgelordUnit.TakeDamage(_enemyUnit.damage);

                edgelordHUD.setHP(_edgelordUnit.currentHP);
                combatText.text = _edgelordUnit.unitName + " took " + _enemyUnit.damage + " damage!";

                Debug.Log("edgelord took dmg");
            }
            else
            {
                _edgelordUnit.TakeDamage(_enemyUnit.damage * 1 / 10);

                combatText.text = _edgelordUnit.unitName + " took " + _enemyUnit.damage * 1 / 10 + " damage!";

                edgelordHUD.setHP(_edgelordUnit.currentHP);
                Debug.Log("edgelord took smaller dmg");
            }
        }

        

        //bool isDead = playerUnit.currentHP == 0
        yield return new WaitForSeconds(2);
        
       
            animationManager.GoblinIdle();
        
    
        
            animationManager.RedknightIdle();
        

        swordguyturn = true;
        catdogturn = true;
        supportgirlturn = true;
        edgelordturn = true;
        if (_playerUnit.currentHP <= 0 && _edgelordUnit.currentHP <= 0 && _supportgirlUnit.currentHP <= 0)
        {
            state = BattleState.LOST;
            StartCoroutine(EndBattle());
        }
        if (_playerUnit.currentHP <= 0 && _edgelordUnit.currentHP <= 0 && _catdogUnit.currentHP <= 0)
        {
            state = BattleState.LOST;
            StartCoroutine(EndBattle());
        }
        if (_playerUnit.Dead() == false)
        {
            PlayerTurn();
        }
        
        state = BattleState.PLAYERTURN;
        swordgBlocking = false;
        sgblocking = false;
        elblocking = false;
        cdblocking = false;

        if (_playerUnit.Dead() == true && supportgirlActive == true && _supportgirlUnit.Dead() == false)
        {
            SupportgirlTurn();
        }
        if (_playerUnit.Dead() == true && supportgirlActive == false && _catdogUnit.Dead() == false)
        {
            CatdogTurn();
        }
        if (_catdogUnit.Dead() == true && _playerUnit.Dead() == true && _edgelordUnit.Dead() == false)
        {
            EdgelordTurn();
        }
        if (_supportgirlUnit.Dead() == true && _playerUnit.Dead() == true && _edgelordUnit.Dead() == false)
        {
            EdgelordTurn();
        }

    }


    IEnumerator EndBattle()
    {
        if (state == BattleState.WON)
        {
            for (int i = 0; i < _button.Length; i++)
            {
                _button[i].SetActive(false);
            }
            
            


            combatText.fontSize = 25;
            Destroy(GameObject.FindGameObjectWithTag("EnemyBattlestation"));
            Debug.Log("enemy disabled");
            combatText.text = "You won the battle!";

            yield return new WaitForSeconds(2);
            for (int i = 0; i < _button.Length; i++)
            {
                _button[i].SetActive(false);
            }
            combatText.fontSize = 25;
            combatText.text = "You gained 0 exp!";
            isDead = false;

            yield return new WaitForSeconds(3);

            SceneManager.LoadScene(Encounter.LastSceneName);


        }
        else if (state == BattleState.LOST)
        {
            for (int i = 0; i < _button.Length; i++)
            {
                _button[i].SetActive(false);
            }

            combatText.fontSize = 25;
            combatText.text = "You lost noob!";
        }
    }
    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;


        StartCoroutine(PlayerAtttack());
        
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;


        StartCoroutine(PlayerHeal());

    }
    public void OnBlockButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;


        StartCoroutine(PlayerBlock());

    }
    public void OnFleeButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;


        StartCoroutine(PlayerFlee());

    }


}*/
