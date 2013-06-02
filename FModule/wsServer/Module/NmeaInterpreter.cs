using System;
using System.Globalization;
public enum NmeaInterpreterEventName
{
    GPRMCReceived,
    GPGSVReceived,
    GPGSAReceived,
    GPGGAReceived,

    GPGGAParsed,
    GPGMCParsed,
    GPGSVParsed,
    GPGSAParsed,

    PositionReceived,
    DateTimeChanged,
    BearingReceived,
    SpeedReceived,
    SpeedLimitReached,
    FixObtained,
    FixLost,
    SatelliteReceived,
    HDOPReceived,
    VDOPReceived,
    PDOPReceived,
    SatellitesInViewReceived,
    SatellitesUsed,
    EllipsoidHeightReceived

}
public class NmeaInterpreter
{
    bool bStop = false;//标识是否停止回调
    // Represents the EN-US culture, used for numers in NMEA sentences
    public static CultureInfo NmeaCultureInfo = new CultureInfo("en-US");
    // Used to convert knots into miles per hour
    public static double MPHPerKnot = double.Parse("1.150779",
      NmeaCultureInfo);
    #region Delegates
    public delegate void PositionReceivedEventHandler(string latitude,
      string longitude);
    public delegate void SearialDataReceivedEventHandler(string strData);
    public delegate void DateTimeChangedEventHandler(System.DateTime dateTime);
    public delegate void BearingReceivedEventHandler(double bearing);
    public delegate void SpeedReceivedEventHandler(double speed);
    public delegate void SpeedLimitReachedEventHandler();
    public delegate void FixObtainedEventHandler();
    public delegate void FixLostEventHandler();
    public delegate void SatelliteReceivedEventHandler(
      int pseudoRandomCode, int azimuth, int elevation,
      int signalToNoiseRatio);
    public delegate void HDOPReceivedEventHandler(double value);
    public delegate void VDOPReceivedEventHandler(double value);
    public delegate void PDOPReceivedEventHandler(double value);
    public delegate void SatellitesInViewReceivedEventHandler(int value);
    public delegate void SatellitesUsedReceivedEventHandler(int value);
    public delegate void EllipsoidHeightReceivedEventHandler(double value);



    #endregion
    //public void AddEventHandler(NmeaInterpreterEventName eventName,object dele)
    //{
    //    this.PositionReceived += (PositionReceivedEventHandler)dele;
    //}
    //public void AddPositionReceivedHandler(PositionReceivedEventHandler PositionReceived_in)
    //{
    //    this.PositionReceived += PositionReceived_in;
    //}
    //public void AddGPRMCReceivedHandler(SearialDataReceivedEventHandler GPRMCReceived_in)
    //{
    //    this.GPRMCReceived += GPRMCReceived_in;
    //}
    //public void RomovePositionReceivedHandler(PositionReceivedEventHandler PositionReceived_in)
    //{
    //    //this.PositionReceived -= PositionReceived_in;
    //    this.PositionReceived = null;
    //}
    //public void RomoveGPRMCReceivedHandler()
    //{
    //    this.GPRMCReceived = null;
    //}
    public void ClearEventsHandler()
    {
        this.GPRMCReceived = null;
        this.GPGSVReceived = null;
        this.GPGSAReceived = null;
        this.GPGGAReceived = null;

        this.GPGGAParsed = null;
        this.GPGMCParsed = null;
        this.GPGSVParsed = null;
        this.GPGSAParsed = null;

        this.PositionReceived = null;
        this.DateTimeChanged = null;
        this.BearingReceived = null;
        this.SpeedReceived = null;
        this.SpeedLimitReached = null;
        this.FixObtained = null;
        this.FixLost = null;
        this.SatelliteReceived = null;
        this.HDOPReceived = null;
        this.VDOPReceived = null;
        this.PDOPReceived = null;
        this.SatellitesInViewReceived = null;
        this.SatellitesUsed = null;
        this.EllipsoidHeightReceived = null;
    }
    #region Events
    public event SearialDataReceivedEventHandler GPRMCReceived;
    public event SearialDataReceivedEventHandler GPGSVReceived;
    public event SearialDataReceivedEventHandler GPGSAReceived;
    public event SearialDataReceivedEventHandler GPGGAReceived;

    public event SearialDataReceivedEventHandler GPGGAParsed;
    public event SearialDataReceivedEventHandler GPGMCParsed;
    public event SearialDataReceivedEventHandler GPGSVParsed;
    public event SearialDataReceivedEventHandler GPGSAParsed;

    public event PositionReceivedEventHandler PositionReceived;
    public event DateTimeChangedEventHandler DateTimeChanged;
    public event BearingReceivedEventHandler BearingReceived;
    public event SpeedReceivedEventHandler SpeedReceived;
    public event SpeedLimitReachedEventHandler SpeedLimitReached;
    public event FixObtainedEventHandler FixObtained;
    public event FixLostEventHandler FixLost;
    public event SatelliteReceivedEventHandler SatelliteReceived;
    public event HDOPReceivedEventHandler HDOPReceived;
    public event VDOPReceivedEventHandler VDOPReceived;
    public event PDOPReceivedEventHandler PDOPReceived;
    public event SatellitesInViewReceivedEventHandler SatellitesInViewReceived;
    public event SatellitesUsedReceivedEventHandler SatellitesUsed;
    public event EllipsoidHeightReceivedEventHandler EllipsoidHeightReceived;
    #endregion

    public void StopInterpreter()
    {
        this.bStop = true;
    }
    public void StartInterpreter()
    {
        this.bStop = false;
    }
    // Processes information from the GPS receiver
    public bool Parse(string sentence)
    {
        // Discard the sentence if its checksum does not match our
        // calculated checksum
        if (!IsValid(sentence))
            return false;
        // Look at the first word to decide where to go next
        switch (GetWords(sentence)[0])
        {
            case "$GPRMC":
                // A "Recommended Minimum" sentence was found!
                if (GPRMCReceived != null && this.bStop == false)
                {
                    GPRMCReceived(sentence);
                }
                return ParseGPRMC(sentence);
            case "$GPGSV":
                // A "Satellites in View" sentence was recieved
                if (GPGSVReceived != null && this.bStop == false)
                {
                    GPGSVReceived(sentence);
                }
                return ParseGPGSV(sentence);
            case "$GPGSA":
                if (null != GPGSAReceived && this.bStop == false)
                {
                    GPGSAReceived(sentence);
                }
                return ParseGPGSA(sentence);
            case "$GPGGA":
                if (null != GPGGAReceived && this.bStop == false)
                {
                    GPGGAReceived(sentence);
                }
                return ParseGPGGA(sentence);
            default:
                // Indicate that the sentence was not recognized
                return false;
        }
    }
    // Divides a sentence into individual words
    public string[] GetWords(string sentence)
    {
        //strip off the final * + checksum
        sentence = sentence.Substring(0, sentence.IndexOf("*"));
        //now split it up
        return sentence.Split(',');
    }
    // Interprets a $GPRMC message
    public bool ParseGPRMC(string sentence)
    {
        string strParsed = "latitude: ";
        string Latitude = string.Empty;
        string Longitude = string.Empty;
        double Speed = -1;
        double Bearing = -1;
        // Divide the sentence into words
        string[] Words = GetWords(sentence);
        // Do we have enough values to describe our location?
        if (Words[3] != "" & Words[4] != "" &
          Words[5] != "" & Words[6] != "")
        {
            // Yes. Extract latitude and longitude
            // Append hours
            Latitude = Words[3].Substring(0, 2) + "°";
            // Append minutes
            Latitude = Latitude + Words[3].Substring(2) + "\"";
            // Append hours
            Latitude = Latitude + Words[4]; // Append the hemisphere
            Longitude = Words[5].Substring(0, 3) + "°";
            // Append minutes
            Longitude = Longitude + Words[5].Substring(3) + "\"";
            // Append the hemisphere
            Longitude = Longitude + Words[6];
            // Notify the calling application of the change
            if (PositionReceived != null && this.bStop == false)
                PositionReceived(Latitude, Longitude);
            strParsed = strParsed + Latitude + " Longitude: " + Longitude;
        }
        // Do we have enough values to parse satellite-derived time?
        if (Words[1] != "")
        {
            // Yes. Extract hours, minutes, seconds and milliseconds
            int UtcHours = Convert.ToInt32(Words[1].Substring(0, 2));
            int UtcMinutes = Convert.ToInt32(Words[1].Substring(2, 2));
            int UtcSeconds = Convert.ToInt32(Words[1].Substring(4, 2));
            int UtcMilliseconds = 0;
            // Extract milliseconds if it is available
            if (Words[1].Length > 7)
            {
                UtcMilliseconds = Convert.ToInt32(Words[1].Substring(7));
            }
            // Now build a DateTime object with all values
            System.DateTime Today = System.DateTime.Now.ToUniversalTime();
            System.DateTime SatelliteTime = new System.DateTime(Today.Year,
              Today.Month, Today.Day, UtcHours, UtcMinutes, UtcSeconds,
              UtcMilliseconds);
            // Notify of the new time, adjusted to the local time zone
            if (DateTimeChanged != null && this.bStop == false)
                DateTimeChanged(SatelliteTime.ToLocalTime());
            strParsed = strParsed + " Time: " + SatelliteTime.ToLocalTime().ToShortTimeString();
        }
        // Do we have enough information to extract the current speed?
        if (Words[7] != "")
        {
            // Yes.  Parse the speed and convert it to MPH
            Speed = double.Parse(Words[7], NmeaCultureInfo) *
              MPHPerKnot;
            // Notify of the new speed
            if (SpeedReceived != null && this.bStop == false)
                SpeedReceived(Speed);
            // Are we over the highway speed limit?
            if (Speed > 55)
                if (SpeedLimitReached != null && this.bStop == false)
                    SpeedLimitReached();
            strParsed = strParsed + " Speed: " + Speed;
        }
        // Do we have enough information to extract bearing?
        if (Words[8] != "")
        {
            // Indicate that the sentence was recognized
            Bearing = double.Parse(Words[8], NmeaCultureInfo);
            if (BearingReceived != null && this.bStop == false)
                BearingReceived(Bearing);
            strParsed = strParsed + " Bearing: " + Bearing.ToString();
        }
        // Does the device currently have a satellite fix?
        if (Words[2] != "")
        {
            switch (Words[2])
            {
                case "A":
                    strParsed = strParsed + " Fixed: True";
                    if (FixObtained != null && this.bStop == false)
                        FixObtained();
                    break;
                case "V":
                    strParsed = strParsed + " Fixed: False";
                    if (FixLost != null && this.bStop == false)
                        FixLost();
                    break;
            }
        }
        // Indicate that the sentence was recognized
        if (null != GPGMCParsed && this.bStop == false)
        {
            GPGMCParsed(strParsed);
        }
        return true;
    }
    // Interprets a "Satellites in View" NMEA sentence
    public bool ParseGPGSV(string sentence)
    {
        string strParsed = "";
        int PseudoRandomCode = 0;
        int Azimuth = 0;
        int Elevation = 0;
        int SignalToNoiseRatio = 0;
        // Divide the sentence into words
        string[] Words = GetWords(sentence);
        // Each sentence contains four blocks of satellite information. 
        // Read each block and report each satellite's information
        int Count = 0;
        for (Count = 1; Count <= 4; Count++)
        {
            // Do we have enough values to parse satellitesIinView?
            if (Words[3] != "")
            {
                if (SatellitesInViewReceived != null && this.bStop == false)
                    SatellitesInViewReceived(int.Parse(Words[3]));
                strParsed = Words[3] + "Satellites In View ";
            }

            // Does the sentence have enough words to analyze?
            if ((Words.Length - 1) >= (Count * 4 + 3))
            {
                // Yes.  Proceed with analyzing the block. 
                // Does it contain any information?
                if (Words[Count * 4] != "" & Words[Count * 4 + 1] != ""
                  & Words[Count * 4 + 2] != "" & Words[Count * 4 + 3] != "")
                {
                    // Yes. Extract satellite information and report it
                    PseudoRandomCode = System.Convert.ToInt32(Words[Count * 4]);
                    Elevation = Convert.ToInt32(Words[Count * 4 + 1]);
                    Azimuth = Convert.ToInt32(Words[Count * 4 + 2]);
                    SignalToNoiseRatio = Convert.ToInt32(Words[Count * 4 + 3]);
                    // Notify of this satellite's information
                    if (SatelliteReceived != null && this.bStop == false)
                        SatelliteReceived(PseudoRandomCode, Azimuth,
                        Elevation, SignalToNoiseRatio);
                    strParsed = strParsed + "PseudoRandomCode: " + PseudoRandomCode.ToString() + " Azimuth: " + Azimuth.ToString() + " Elevation: " + Elevation + " NoiseRatio: " + SignalToNoiseRatio.ToString();
                }
            }
        }
        // Indicate that the sentence was recognized
        if (null != GPGSVParsed && this.bStop == false)
        {
            GPGSVParsed(strParsed);
        }
        return true;
    }
    // Interprets a "Fixed Satellites and DOP" NMEA sentence
    public bool ParseGPGSA(string sentence)
    {
        // Divide the sentence into words
        string[] Words = GetWords(sentence);
        // Update the DOP values
        if (Words[15] != "")
        {
            if (PDOPReceived != null && this.bStop == false)
                PDOPReceived(double.Parse(Words[15], NmeaCultureInfo));
        }
        if (Words[16] != "")
        {
            if (HDOPReceived != null && this.bStop == false)
                HDOPReceived(double.Parse(Words[16], NmeaCultureInfo));
        }
        if (Words[17] != "")
        {
            if (VDOPReceived != null && this.bStop == false)
                VDOPReceived(double.Parse(Words[17], NmeaCultureInfo));
        }
        return true;
    }
    // Returns True if a sentence's checksum matches the

    //  Interprets a $GPGGA message
    public bool ParseGPGGA(string sentence)
    {
        // Divide the sentence into words
        string[] Words = GetWords(sentence);
        // Satellites Used
        if (Words[7] != "")
        {
            if (SatellitesUsed != null && this.bStop == false)
                SatellitesUsed(int.Parse(Words[7]));
        }
        if (Words[8] != "")
        {
            if (HDOPReceived != null && this.bStop == false)
                HDOPReceived(double.Parse(Words[8], NmeaCultureInfo));
        }

        if (Words[9] != "")
        {
            if (EllipsoidHeightReceived != null && this.bStop == false)
                EllipsoidHeightReceived(double.Parse(Words[9]));
        }
        //

        return true;
    }
    // Returns True if a sentence's checksum matches the

    // calculated checksum
    public bool IsValid(string sentence)
    {
        // Compare the characters after the asterisk to the calculation
        string checksumLocal = GetChecksum(sentence);
        string temp = sentence.Substring(sentence.IndexOf("*") + 1);
        string checksum = temp;
        int indexOfSpace = temp.IndexOf(" ");
        if (indexOfSpace > 0)
        {
            checksum = sentence.Substring(sentence.IndexOf("*") + 1, indexOfSpace);
        }
        return checksumLocal == checksum;
    }
    // Calculates the checksum for a sentence
    public string GetChecksum(string sentence)
    {
        // Loop through all chars to get a checksum
        int Checksum = 0;
        foreach (char Character in sentence)
        {
            if (Character == '$')
            {
                // Ignore the dollar sign
            }
            else if (Character == '*')
            {
                // Stop processing before the asterisk
                break;
            }
            else
            {
                // Is this the first value for the checksum?
                if (Checksum == 0)
                {
                    // Yes. Set the checksum to the value
                    Checksum = Convert.ToByte(Character);
                }
                else
                {
                    // No. XOR the checksum with this character's value
                    Checksum = Checksum ^ Convert.ToByte(Character);
                }
            }
        }
        // Return the checksum formatted as a two-character hexadecimal
        return Checksum.ToString("X2");
    }
}
