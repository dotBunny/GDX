using System;
using UnityEngine;

namespace GDX.Developer.Reports.BuildVerification
{
    public abstract class TestBehaviour : MonoBehaviour
    {
        protected abstract void Check();
        protected abstract string GetIdentifier();
        void Start()
        {
            try
            {
                Check();
            }
            catch(Exception ex)
            {
                BVT.Assert(GetIdentifier(), false, ex.Message);
            }
        }
    }
}