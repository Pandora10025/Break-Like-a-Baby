using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

//[ExecuteInEditMode]


public class BoxRockerTest : MonoBehaviour
{




    [SerializeField] Vector3 rotationPoint;
    public Material[] hitMaterials;



    [SerializeField] Vector3 hitPoint;
    [SerializeField] Vector3 hitDirection = new Vector3(1, 0, 1);

    [SerializeField] Vector2 floorRectScale = new Vector2(1, 1);


    private Vector3 tiltCorner;

    private float TAU = 6.28318530718f;


    [SerializeField] int bouncesNumber = 2;

    [SerializeField] float bounceWeight = .5f;

    [SerializeField] float bounceIntensity = 2;

    [Range(0, 1)]
    public float animProgress = 0;

    private float animVelocity = 0;

    [SerializeField] float rotationAmount = 45;

    [SerializeField] float shakeSpeed = 1;


    private bool currentShaking = false;

    public bool testShake = false;



    private float outlineGlowDist = 3.5f;

    private float outlineChangeProgress = 1;




    void Start()
    {
        ////Clone materials, so that we have individual instances!

        //for (int i = 0; i < hitMaterials.Length; i++)
        //{
        //    Material cloneMaterial = new Material(hitMaterials[i]);


        //    hitMaterials[i] = cloneMaterial;


        //}





    }

    // Update is called once per frame
    void Update()
    {

        //I'm going to make it so that the outliens glow different colors to signify the importance of the breakable objects
        //...or they are just black when the player is far away.


        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");


        bool foundPlayer = false;

        for (int i = 0; i < players.Length; i++)
        {
            GameObject currentPlayer = players[i];

            if (Vector3.Distance(transform.position, currentPlayer.transform.position) < outlineGlowDist)
            {
                foundPlayer = true;


                
            }


        }




        if (foundPlayer)
        {
            outlineChangeProgress = Mathf.Lerp(outlineChangeProgress, 1, .3f);

        }
        else
        {
            outlineChangeProgress = Mathf.Lerp(outlineChangeProgress, 0, .3f);


        }

        Color currentOutlineGlowColor = Color.Lerp(Color.black, Color.Lerp(Color.red, Color.yellow, (Mathf.Sin(Time.realtimeSinceStartup * 1)+1)/2    ), outlineChangeProgress);





        //And then down here it's time for the actual rotation calculations. We'll set the color in just a bit.









        Vector3 planeCenter = transform.position - transform.up / 2 * transform.lossyScale.y;

        Vector3 frontRightDir = transform.rotation * new Vector3(1 * floorRectScale.x * .5f * transform.lossyScale.x, 0, 1 * floorRectScale.y * .5f * transform.lossyScale.z);
        Vector3 frontLeftDir = transform.rotation * new Vector3(-1 * floorRectScale.x * .5f * transform.lossyScale.x, 0, 1 * floorRectScale.y * .5f * transform.lossyScale.z);
        Vector3 backRightDir = transform.rotation * new Vector3(1 * floorRectScale.x * .5f * transform.lossyScale.x, 0, -1 * floorRectScale.y * .5f * transform.lossyScale.z);
        Vector3 backLeftDir = transform.rotation * new Vector3(-1 * floorRectScale.x * .5f * transform.lossyScale.x, 0, -1 * floorRectScale.y * .5f * transform.lossyScale.z);

        Vector3 frontRightCorner = planeCenter + frontRightDir;
        Vector3 frontLeftCorner = planeCenter + frontLeftDir;
        Vector3 backRightCorner = planeCenter + backRightDir;
        Vector3 backLeftCorner = planeCenter + backLeftDir;

        Vector3[] cornersToCheck = { frontRightCorner, frontLeftCorner, backRightCorner, backLeftCorner };


        if (Vector3.Dot((transform.position - hitPoint).normalized, hitDirection.normalized) < 0)
        {
            hitDirection *= -1;
        }


        //The corner we pick will be the farthest corner with the largest dot product.
        Vector3 closestCorner = Vector3.zero;
        float closestDist = 0;
        float closestDot = 0;


        for (int i = 0; i < 4; i++)
        {

            Vector3 currentCorner = cornersToCheck[i];

            float currentDist = Vector3.Distance(currentCorner, hitPoint);
            float currentDot = Mathf.Max(Vector3.Dot(hitDirection.normalized, (currentCorner - hitPoint).normalized), 0);


            if (currentDist * currentDot >= closestDist * closestDot)
            {

                closestCorner = currentCorner;
                closestDist = currentDist;
                closestDot = currentDot;

            }



        }


        rotationPoint = closestCorner;




        if (testShake)
        {


            testShake = false;

            Shake(transform.position + new Vector3(0,0,3), transform.right);

        }


        for (int i = 0; i < hitMaterials.Length; i++)
        {

            Material hitMaterial = hitMaterials[i];


            Vector3 currentCenter = hitMaterial.GetVector("_Center");
            Vector3 centeredCenter = rotationPoint - transform.position;
            Vector3 lerpedCenter = Vector3.Lerp(currentCenter, centeredCenter, 0.1f);





            Vector3 rotationAxis = Vector3.Cross(Vector3.up, hitDirection).normalized;
            Vector3 currentRotationAxis = hitMaterial.GetVector("_ForwardAxis");
            Vector3 slerpedRotationAxis = Vector3.Slerp(currentRotationAxis, rotationAxis, 0.1f);







            hitMaterial.SetVector("_UpAxis", new Vector4(transform.up.x, transform.up.y, transform.up.z, 0));
            //hitMaterial.SetVector("_ForwardAxis",  new Vector4( transform.forward.x , transform.forward.y , transform.forward.z  , 0  )   );
            //hitMaterial.SetVector("_ForwardAxis",  new Vector4( rotationAxis.x , rotationAxis.y , rotationAxis.z  , 0  )   );
            hitMaterial.SetVector("_ForwardAxis", new Vector4(slerpedRotationAxis.x, slerpedRotationAxis.y, slerpedRotationAxis.z, 0));
            hitMaterial.SetVector("_Center", new Vector4(lerpedCenter.x, lerpedCenter.y, lerpedCenter.z, 0));

            hitMaterial.SetColor("_OutlineColor", currentOutlineGlowColor   );



            if (currentShaking)
            {
                animProgress += Time.deltaTime * shakeSpeed * animVelocity;

                //animVelocity = Mathf.Lerp( animVelocity, 1, .08f );


                if (animProgress < 0)
                {
                    animProgress *= -1;

                    animVelocity = 1;



                }
                else if (animProgress > 1)
                {

                    animProgress = 0;

                    animVelocity = 0;

                    currentShaking = false;
                }
            }








            hitMaterial.SetFloat("_rotY", rotationAmount / 360f * customAnimation(animProgress));





        }












    }

    public void Shake(Vector3 playerPos, Vector3 playerRightVector)
    {
        currentShaking = true;

        hitPoint = playerPos;

        hitDirection = playerRightVector;



        if (animVelocity == 0)
        {
            animVelocity = 1;


        }
        else
        {

            animVelocity *= -1;

        }





    }




    private float customAnimation(float progress)
    {
        //This function is designed to output a range of 0-1, so that you can use that as a percent.
        //Keep in mind that this funciton isn't linear at all--it goes up and down quite a bit!
        //That's so that it can bounce.



        progress = Mathf.Clamp01(progress);


        float bounceNumberForCalculation = (float)(bouncesNumber + 1);

        float b = bounceNumberForCalculation;

        float sineWave = Mathf.Abs(Mathf.Sin(b * Mathf.PI * Mathf.Pow(1 - progress, bounceWeight)));

        float sineScaling = Mathf.Pow(Mathf.Ceil(b * Mathf.Pow(1 - progress, bounceWeight)) / b, bounceIntensity);




        return sineWave * sineScaling;

    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawSphere(rotationPoint, .25f);



        Vector3 rotationAxis = Vector3.Cross(Vector3.up, hitDirection).normalized;
        Gizmos.DrawLine(hitPoint + hitDirection, hitPoint + hitDirection + rotationAxis);


        Gizmos.color = Color.black;

        Gizmos.DrawSphere(hitPoint, .25f);

        Gizmos.DrawLine(hitPoint, hitPoint + hitDirection);





        //Now to calculate our own little floor-plane

        Vector3 planeCenter = transform.position - transform.up / 2 * transform.lossyScale.y;

        Gizmos.DrawWireSphere(planeCenter, .25f);


        //But the real trick here is that we're going to draw a circle, and then use trig to turn that into a square based on the rotation from the forward direction.
        //While this is not normally the optimal way to draw a square (normally just pick the corners) it will help us do some absolutely ridiculous and cool automation stuff later on.
        //So we'd better just draw our circle first!

        int arcSteps = 32;


        for (int i = 0; i < arcSteps; i++)
        {
            float currentPercent = (float)i / (float)arcSteps;
            float nextPercent = (float)(i + 1) / (float)arcSteps;

            //Quaternion from = transform.rotation * Quaternion.Euler(0, currentRadians, 0); 
            //Quaternion to = transform.rotation * Quaternion.Euler(0, nextRadians, 0);

            Vector3 from = Quaternion.Euler(0, currentPercent * 360, 0) * Vector3.forward;
            Vector3 to = Quaternion.Euler(0, nextPercent * 360, 0) * Vector3.forward;



            Vector3 fromForward = transform.rotation * from;
            Vector3 toForward = transform.rotation * to;





            Gizmos.DrawLine(planeCenter + fromForward * .5f, planeCenter + toForward * .5f);

        }



        //Now to generate the corners!

        Vector3 frontRightDir = transform.rotation * new Vector3(1 * floorRectScale.x * .5f * transform.lossyScale.x, 0, 1 * floorRectScale.y * .5f * transform.lossyScale.z);
        Vector3 frontLeftDir = transform.rotation * new Vector3(-1 * floorRectScale.x * .5f * transform.lossyScale.x, 0, 1 * floorRectScale.y * .5f * transform.lossyScale.z);
        Vector3 backRightDir = transform.rotation * new Vector3(1 * floorRectScale.x * .5f * transform.lossyScale.x, 0, -1 * floorRectScale.y * .5f * transform.lossyScale.z);
        Vector3 backLeftDir = transform.rotation * new Vector3(-1 * floorRectScale.x * .5f * transform.lossyScale.x, 0, -1 * floorRectScale.y * .5f * transform.lossyScale.z);

        Vector3 frontRightCorner = planeCenter + frontRightDir;
        Vector3 frontLeftCorner = planeCenter + frontLeftDir;
        Vector3 backRightCorner = planeCenter + backRightDir;
        Vector3 backLeftCorner = planeCenter + backLeftDir;


        Gizmos.DrawLine(frontRightCorner, frontLeftCorner);
        Gizmos.DrawLine(frontLeftCorner, backLeftCorner);
        Gizmos.DrawLine(backLeftCorner, backRightCorner);
        Gizmos.DrawLine(backRightCorner, frontRightCorner);























    }


}
