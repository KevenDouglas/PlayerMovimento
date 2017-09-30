//configurando a movimentação do player.
//author: Keven Douglas.
//github: kevendouglas.github.io.

using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace CompleteProject
{
    public class PlayerMovimento : MonoBehaviour
    {
        public float speed = 5f;            // A velocidade em que o jogador se moverá


        Vector3 movement;                   // O vetor para armazenar a direção do movimento do jogador.
        Animator anim;                      // Referência ao componente animador.
        Rigidbody playerRigidbody;          // Referência ao rígido do jogador.	
#if !MOBILE_INPUT
        int floorMask;                       // Uma máscara de camada para que um raio possa ser lançado apenas em objetos de jogo na camada de chão.
        float camRayLength = 100f;          // O comprimento do raio da câmera para a cena.

#endif

        void Awake ()
        {
#if !MOBILE_INPUT
           // Crie uma máscara de camada para a camada do chão.
            floorMask = LayerMask.GetMask ("Floor");
#endif

            // Configurar referências.
            anim = GetComponent <Animator> ();
            playerRigidbody = GetComponent <Rigidbody> ();
        }


        void FixedUpdate ()
        {
            // Armazena os eixos de entrada.
            float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            float v = CrossPlatformInputManager.GetAxisRaw("Vertical");

            // Mova o jogador ao redor da cena.
            Move (h, v);

            // Gire o jogador para enfrentar o cursor do mouse.
            Turning ();

            // Animar o jogador.
            Animating (h, v);
        }


        void Move (float h, float v)
        {
            // Defina o vetor de movimento com base na entrada do eixo.
            movement.Set (h, 0f, v);
            
            // Normalize o vetor de movimento e o torna proporcional à velocidade por segundo.
            movement = movement.normalized * speed * Time.deltaTime;

            // Move o jogador para a posição atual mais o movimento.
            playerRigidbody.MovePosition (transform.position + movement);
        }


        void Turning ()
        {
#if !MOBILE_INPUT
            // Crie um raio do cursor do mouse na tela na direção da câmera.
            Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

            // Crie uma variável RaycastHit para armazenar informações sobre o que foi atingido pelo raio.
            RaycastHit floorHit;

            // Execute o raycast e se atingir algo na camada do chão ...
            if(Physics.Raycast (camRay, out floorHit, camRayLength, floorMask))
            {
                // Crie um vetor do jogador até o ponto no chão, o alcance do mouse do mouse.
                Vector3 playerToMouse = floorHit.point - transform.position;

                // Verifique se o vetor está inteiramente ao longo do plano do chão.
                playerToMouse.y = 0f;

                // Crie um quaternion (rotação) com base em olhar para baixo o vetor do jogador para o mouse.
                Quaternion newRotatation = Quaternion.LookRotation (playerToMouse);

                // Defina a rotação do jogador para esta nova rotação.
                playerRigidbody.MoveRotation (newRotatation);
            }
#else

            Vector3 turnDir = new Vector3(CrossPlatformInputManager.GetAxisRaw("Mouse X") , 0f , CrossPlatformInputManager.GetAxisRaw("Mouse Y"));

            if (turnDir != Vector3.zero)
            {
                // Crie um vetor do jogador até o ponto no chão, o alcance do mouse do mouse.
                Vector3 playerToMouse = (transform.position + turnDir) - transform.position;

                // Verifique se o vetor está inteiramente ao longo do plano do chão.
                playerToMouse.y = 0f;

                // Crie um quaternion (rotação) com base em olhar para baixo o vetor do jogador para o mouse.
                Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);

                // Defina a rotação do jogador para esta nova rotação.
                playerRigidbody.MoveRotation(newRotatation);
            }
#endif
        }


        void Animating (float h, float v)
        {
            // Crie um booleano que seja verdadeiro se qualquer um dos eixos de entrada for diferente de zero.
            bool walking = h != 0f || v != 0f;

            // Diga ao animador se o jogador está ou não andando.
            anim.SetBool ("IsWalking", walking);
        }
    }
}
