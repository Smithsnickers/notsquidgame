namespace OpenCvSharp.Demo
{
    using System.Collections;
    using System.Collections.Generic;
    using OpenCvSharp.Demo;
    using UnityEngine;
    using System.Linq;
    using System;
    using System.Runtime.CompilerServices;
    using System.Diagnostics;

    class Game: MonoBehaviour
    {
        private List<DetectedFace> AllPlayer = new List<DetectedFace>();
        private FaceDetectorScene FaceDetector;

        // Start is called before the first frame update
        void Start()
        {
            GameObject cameraObject = GameObject.Find("RawImage");
            FaceDetector = (FaceDetectorScene) cameraObject.GetComponent(typeof(FaceDetectorScene));            
        }

        // Update is called once per frame
        void Update()
        {
            foreach (DetectedFace face in FaceDetector.Faces()){
                AddPlayer(face);
            }
        }

        bool AddPlayer(DetectedFace newPlayer)
        {
            if(!EmptyPlayer(newPlayer.Region)){
                if(!SamePlayer(newPlayer.Region)){
                    UnityEngine.Debug.Log("New player at: "+newPlayer.Region+" currently there are "+AllPlayer.Count+" playing");
                    AllPlayer.Add(newPlayer);
                    return true;
                } 
            }            

            return false;
        }

        private bool SamePlayer(OpenCvSharp.Rect newPlayer)
        {
            var sorted = AllPlayer.Select(player => new
            {
                distance = (Math.Pow(player.Region.X-newPlayer.X,2)+Math.Pow(player.Region.Y-newPlayer.Y,2)),
                face = player
            })
            .OrderBy(player => player.distance).Where(player => player.distance < 50);

            if(sorted.Any()){                                
                int playerIndex = AllPlayer.FindIndex(player => player == sorted.First().face);            
                AllPlayer[playerIndex].SetRegion(newPlayer);
                UnityEngine.Debug.Log("Moved player to: "+newPlayer+" currently there are "+AllPlayer.Count+" playing");
            }            

            return sorted.Any() ? true : false;
        }

        private bool EmptyPlayer(OpenCvSharp.Rect newPlayer)
        {
            return newPlayer.X == 0 && newPlayer.Y == 0;
        }
    }
}