using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using PrimoVictoria.Models;

namespace PrimoVictoria.DataModels
{
    /// <summary>
    /// Stands represent the trays on which a model stands.  They hold X number of models
    /// </summary>
    public class Stand : MonoBehaviour
    {
        public Unit ParentUnit;
        public Guid Id;
        public List<GameObject> ModelMeshes; //the living meshes
        public int StandCapacity;  //how many models fit on the stand at full capacity
        public bool StandVisible;  //if true will draw the stand that the models are on, if false will not (mostly useful for debugging / dev work)
                                   //todo: add a stand object that they can stand on that can be turned off and on

        private void Update()
        {

        }
    }
}
