﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SpaceXSimCSharpTest
{

    class Stage
    {
        #region Fields
        //Bipropellant tanks
        private Tank kerosene;
        private Tank oxygen;
        //Holds the various control inputs of the rocket, not implemented
        ControlSystem[] stageControl;
        //Determines stage characteristics
        Stage_Type type;
        //Holds the engines of the stage
        OctoWeb engineStructure;
        #endregion

        #region Properties
        public Tank Kerosene
        {
            get { return kerosene; }
        }

        public Tank Oxygen
        {
            get { return oxygen; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a stage with dynamically sized tanks based on stage type
        /// </summary>
        /// <param name="type">Type of stage this is</param>
        public Stage(Stage_Type type)
        {
            //2.56 to 1 LOX to RP1
            this.type = type;
            switch (type)
            {
                case Stage_Type.firstStage:
                    #region FirstStage
                    /*
                    kerosene = new Tank(10.477, Global.RADIUS, Fuel_Type.RP1);
                    oxygen = new Tank(26.822, Global.RADIUS, Fuel_Type.LO2);
                    

                    kerosene = new Tank(12.996, Global.RADIUS, Fuel_Type.RP1);
                    oxygen = new Tank(24.304, Global.RADIUS, Fuel_Type.LO2);
                    */
                    kerosene = new Tank(Global.FIRSTSTAGELENGTHRP1, Global.RADIUS, Fuel_Type.RP1);
                    oxygen = new Tank(Global.FIRSTSTAGELENGTHLO2, Global.RADIUS, Fuel_Type.LO2);
                    stageControl = new ControlSystem[3];
                    engineStructure = new OctoWeb(type);
                    // engineStructure= new OctoWeb()
                    #endregion
                    break;
                case Stage_Type.secondStage:
                    #region SecondStage
                    /*
                    kerosene = new Tank(3.932, Global.RADIUS, Fuel_Type.RP1);
                    oxygen = new Tank(10.068, Global.RADIUS, Fuel_Type.LO2);
                    
                    kerosene = new Tank(4.464, Global.RADIUS, Fuel_Type.RP1);
                    oxygen = new Tank(10.536, Global.RADIUS, Fuel_Type.LO2);
                    
                    kerosene = new Tank(5.190, Global.RADIUS, Fuel_Type.RP1);
                    oxygen = new Tank(9.81, Global.RADIUS, Fuel_Type.LO2);
                     * */
                    kerosene = new Tank(Global.SECONDSTAGELENGTHRP1, Global.RADIUS, Fuel_Type.RP1);
                    oxygen = new Tank(Global.SECONDSTAGELENGTHLO2, Global.RADIUS, Fuel_Type.LO2);
                    stageControl = new ControlSystem[2];
                    engineStructure = new OctoWeb(type);
                    #endregion
                    break;
            }


        }
        #endregion

        #region Methods
        #region Deprecated ThreadFill
        //Deprecated, currently implemented in ThreadManager
        /*
        /// <summary>
        /// Creates two threads to fill up both propellant tanks
        /// </summary>
        public void FillThreaded()
        {
            Thread fill1 = new Thread(this.FillLO2);
            Thread fill2 = new Thread(this.FillRP1);
            fill1.Start();
            fill2.Start();
            fill2.Join();

        }

        */
        /// <summary>
        /// Fills up a LO2 tank
        /// </summary>
        #endregion

        #region FillLO2
        //Fills the tank with a volume determined by the fill rate and timestep
        //fill rate is seconds, timestep is fractional seconds
        public void FillLO2()
        {
            if (oxygen.FilledVolume < oxygen.MaxVolume)
            {
                oxygen.Fill(Global.LO2FillRate*Global.TIMESTEP);
            }
        }
        #endregion

        #region FillRP1()
        //Fills the tank with a volume determined by the fill rate and timestep
        //fill rate is seconds, timestep is fractional seconds
        public void FillRP1()
        {
            if (kerosene.FilledVolume < kerosene.MaxVolume)
            {
                kerosene.Fill(Global.RP1FillRate*Global.TIMESTEP);
            }
        }
        #endregion

        #region IsFilled()
        //returns a boolean if all tanks in stage are filled
        public bool IsFilled()
        {
            if (oxygen.FilledVolume >= oxygen.MaxVolume && kerosene.FilledVolume >= kerosene.MaxVolume)
            {
                //Console.WriteLine("isFilled is true" + type.ToString());
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region FireEngines()
        /// <summary>
        /// Updates all engines in the stage, drawing fuel and updating thrust
        /// </summary>
        public void FireEngines()
        {
            //Console.WriteLine("engines have been fired");
            for (int i = 0; i < engineStructure.merlinStack.Length; i++)
            {
                engineStructure.merlinStack[i].Update(this);
            }
        
        }
        #endregion

        #region TotalThrust()
        /// <summary>
        /// Returns the total thrust of all engines combined
        /// </summary>
        /// <returns></returns>
        public double TotalThrust()
        {
            double total = 0;
            for (int i = 0; i < engineStructure.merlinStack.Length; i++)
            {
                total += engineStructure.merlinStack[i].Thrust;
            }
            return total;
        }
        #endregion

        #region SingleThrust()
        /// <summary>
        /// Returns the current thrust of a single engine in the stage
        /// </summary>
        /// <returns></returns>
        public double SingleThrust()
        {
            return engineStructure.merlinStack[1].Thrust;
        }
        #endregion

        #region TotalMass()
        /// <summary>
        /// Returns the total mass of the Stage, including all tanks and structures
        /// </summary>
        /// <returns></returns>
        public double TotalMass()
        {
            return oxygen.Mass + kerosene.Mass + engineStructure.Mass;
        }
        #endregion

        #region ChangeThrottle
        /// <summary>
        /// Changes the throttle value by the specified amount for every engine in the stage
        /// </summary>
        /// <param name="v"></param>
        public void ChangeThrottle(double v)
        {
            for (int i = 0; i < engineStructure.merlinStack.Length; i++)
            {
                engineStructure.merlinStack[i].ChangeThrottle(v);
            }
        }
        #endregion
        #endregion
    }
}
