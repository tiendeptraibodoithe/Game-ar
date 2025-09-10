    using UnityEngine;
    using System.Collections;

    public class BirdMovement : MonoBehaviour {

        Vector3 birdInitPosition;
	    public float flapSpeed = 5f;
	    public bool didFlap = false;
	    public Vector3 jumpForce;
	    Rigidbody rigidBody;
	    GameManager mainMan;
        public Transform birdAnchor;

        private AudioSource audioSource;
        public AudioClip flapSound;
        public AudioClip hitSound;
        public AudioClip scoreSound;

        [SerializeField] ParticleSystem boostParticles;

        private bool hitGround = false;

        void Start(){
            rigidBody = GetComponent<Rigidbody> ();
            rigidBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            audioSource = GetComponent<AudioSource>();
            birdInitPosition = this.transform.localPosition;
		    mainMan = GameObject.Find ("GameManager").GetComponent<GameManager> ();
	    }

	    void Update () {
            if (mainMan.startedGame){
                rigidBody.useGravity = true;

                if (mainMan.stillAlive) {
                    // Jump
                    if (Input.GetKeyUp("space") || Input.GetMouseButtonUp(0)) {
                        rigidBody.velocity = Vector3.zero;
                        rigidBody.AddForce(jumpForce);
                        audioSource.PlayOneShot(flapSound);
                        boostParticles.Play();
                }

                    if (rigidBody.velocity.y > 0)
                        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, 0);
                    //} else {
                    //    float angle = Mathf.Lerp(0, -90.0f, -rigidBody.velocity.y / 2);
                    //    transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, angle);
                    //}

                }
            }else {
                rigidBody.useGravity = false;
            }
	    }

    public void ResetBird()
    {
        print(birdInitPosition);

        // Spawn bird như con của BirdAnchor
        if (birdAnchor != null)
        {
            transform.SetParent(birdAnchor, false); // false = giữ nguyên local transform
            transform.localPosition = birdInitPosition;
        }
        else
        {
            transform.localPosition = birdInitPosition;
        }

        transform.localRotation = Quaternion.identity;
        rigidBody.useGravity = false;

        // Giữ bird trong mặt phẳng X-Y, không cho lệch trục Z
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation |
                                RigidbodyConstraints.FreezePositionZ |
                                RigidbodyConstraints.FreezePositionX;

        rigidBody.velocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision collision) {
		    if (collision.gameObject.tag == "wall" || collision.gameObject.tag == "floor") {
                audioSource.PlayOneShot(hitSound);
                mainMan.stillAlive = false;
            }
            rigidBody.constraints = RigidbodyConstraints.None;
	    }

        private void OnTriggerEnter(Collider other){
            if (other.gameObject.tag == "CounterTrigger"){
                print("Collided with points");
                audioSource.PlayOneShot(scoreSound);
                Destroy(other);
                mainMan.AddPoints(1);
            }
        }
    }
