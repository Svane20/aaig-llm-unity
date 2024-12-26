using UnityEngine;

namespace GameSystems.Combat
{
    public class AnimationManager : MonoBehaviour
    {
        public Animator swordguy;
        public Animator supportgirl;
        public Animator edgelord;
        public Animator catdog;

        public Animator enemy;
    

        public Animator earthElemental;

        public Animator explosion;
    
        public Transform playerBattlestation;

        public void Start()
        {
        
        
            GameObject Swordguy = GameObject.FindGameObjectWithTag("Swordguy");
            if (Swordguy == isActiveAndEnabled)
            {
                swordguy = Swordguy.GetComponent<Animator>();
            }
            
        
            GameObject Edgelord = GameObject.FindGameObjectWithTag("Edgelord");
            if (Edgelord == isActiveAndEnabled)
            {
                edgelord = Edgelord.GetComponent<Animator>();
            }

            enemy = GameObject.FindGameObjectWithTag("EnemyBattlestation").GetComponentInChildren<Animator>();
        
        
            GameObject Supportgirl = GameObject.FindGameObjectWithTag("Supportgirl");
            if (Supportgirl == isActiveAndEnabled)
                supportgirl = Supportgirl.GetComponent<Animator>();
        
        
        
            
            GameObject Catdog = GameObject.FindGameObjectWithTag("Catdog");
            if (Catdog == isActiveAndEnabled)
                catdog = Catdog.GetComponent<Animator>();
        
        
        
        
        
        
        
        }

        /*private void LateStart()
    {
        
       
        
        
        
        
        
    }*/

    

        public void EarthElementalAttack()
        {
            earthElemental.Play("Earth_Elemental");
            //earthElemental.Play("EarthElemental_Attack");
        }

        public void EarthElementalIdle()
        {
            earthElemental.Play("EarthElemental_Idle");
        }

        public void Explosion()
        {
            explosion.Play("Explosion_Attack");
        }

        public void ExplosionIdle()
        {
            explosion.Play("Explosion_Idle");
        }

        public void SwordguyAttack()
        {
        
            swordguy.Play("Swordguy_Attack");
        
            swordguy.Play("Swordguy_Idle");
        }

        public void SwordguyIdle()
        {
            swordguy.Play("Swordguy_Idle");
        }

        public void SupportgirlIdle()
        {
            supportgirl.Play("Supportgirl_Idle");
        }

        public void SupportgirlAttack()
        {
            supportgirl.Play("Supportgirl_Attack");
        }

        public void EdgelordAttack()
        {
            edgelord.Play("Edgelord_Attack");
        }

        public void EdgelordIdle()
        {
            edgelord.Play("Edgelord_Idle");
        }

        public void CatdogIdle()
        {
            catdog.Play("Catdog_Idle");
        }

        public void CatdogAttack()
        {
            catdog.Play("Catdog_Attack");
        }
    
    
    
    

        public void GoblinIdle()
        {
            enemy.Play("Goblin_Idle");
        }

        public void GoblinAttack1()
        {
            enemy.Play("Goblin_attack1");
        }
        public void GoblinAttack2()
        {
            enemy.Play("Goblin_Attack2");
        }
        public void GoblinAttack3()
        {
            enemy.Play("Goblin_Attack3");
        }
    
    
    
        public void RedknightIdle()
        {
            enemy.Play("Redknight_Idle");
        }

        public void RedknightAttack1()
        {
            enemy.Play("Redknight_Attack1(2)");
        }
        public void RedknightAttack2()
        {
            enemy.Play("Redknight_Attack2");
        }
        public void RedknightAttack3()
        {
            enemy.Play("Redknight_Attack3");
        }
    }
}
