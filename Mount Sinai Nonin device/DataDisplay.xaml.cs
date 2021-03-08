using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Mount_Sinai_Nonin_device
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
  
    
    public sealed partial class DataDisplay : Page
    {
        private MainPage rootPage = MainPage.Current;

        private BluetoothLEDevice bluetoothLeDevice = null;
        private GattCharacteristic selectedCharacteristic;

        // Only one registered characteristic at a time.
        private GattCharacteristic registeredCharacteristic;
        private GattPresentationFormat presentationFormat;

        //heartRete value
        private GattCharacteristic HartRateCharacteristic;
        GattCharacteristic HRCTag;
        String heartrateShare =""; 

        //battery level 
        private GattCharacteristic BatteryCharacteristic;
        GattCharacteristic BLCTag;


        //SpO2 value 
        private GattCharacteristic spO2Characteristic;
        GattCharacteristic SPO2CTag;
        String SpO2Share = "";

        //Pulse Interval Timing(PIT)
        private GattCharacteristic PITCharacteristic;
        GattCharacteristic PITCTag;

        public DataDisplay()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SelectedDeviceRun.Text = rootPage.SelectedBleDeviceName;
            if (string.IsNullOrEmpty(rootPage.SelectedBleDeviceId))
            {
                ConnectButton.IsEnabled = false;
            }

            //use for data sharing 
            DataTransferManager dtm = DataTransferManager.GetForCurrentView();
            dtm.DataRequested += Dtm_DataRequested;

        }

        private void Dtm_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            
           //TODO if device dose not connect then can not be shared
           // request.FailWithDisplayText("some data dose not retrive, please try again")

            request.Data.Properties.Title = "Nonin 3150 device";
            request.Data.Properties.Description = "sharing your data to mount sinai";
            request.Data.SetText("data come from device: " + rootPage.SelectedBleDeviceName +"\n SpO2 data" + SpO2Share +"\n heart rate data: " + heartrateShare);
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            var success = await ClearBluetoothLEDeviceAsync();
            if (!success)
            {
               // rootPage.NotifyUser("Error: Unable to reset app state", NotifyType.ErrorMessage);
            }

            //use for data sharing but no longer shared
            DataTransferManager dtm = DataTransferManager.GetForCurrentView();
            dtm.DataRequested -= Dtm_DataRequested;

        }

        private void ShareData(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();

        }

        private async Task<bool> ClearBluetoothLEDeviceAsync()
        {
            if (subscribedForNotifications)
            {
                // Need to clear the CCCD from the remote device so we stop receiving notifications
                var result = await registeredCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                if (result != GattCommunicationStatus.Success)
                {
                    return false;
                }
                else
                {
                    //selectedCharacteristic.ValueChanged -= Characteristic_ValueChanged;
                    subscribedForNotifications = false;
                }
            }
            bluetoothLeDevice?.Dispose();
            bluetoothLeDevice = null;
            return true;
        }

        private async void ConnectButton_Click()
        {
            ConnectButton.IsEnabled = false;

            if (!await ClearBluetoothLEDeviceAsync())
            {
                //rootPage.NotifyUser("Error: Unable to reset state, try again.", NotifyType.ErrorMessage);
                ConnectButton.IsEnabled = true;
                return;
            }

            try
            {
                // BT_Code: BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
                bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(rootPage.SelectedBleDeviceId);

                if (bluetoothLeDevice == null)
                {
                 //   rootPage.NotifyUser("Failed to connect to device.", NotifyType.ErrorMessage);
                }
            }
            catch (Exception ex) // when (ex.HResult == E_DEVICE_NOT_AVAILABLE)
            {
                //rootPage.NotifyUser("Bluetooth radio is not on.", NotifyType.ErrorMessage);
            }

            if (bluetoothLeDevice != null)
            {
                GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);
                
                if (result.Status == GattCommunicationStatus.Success)
                {
                    var services = result.Services;

                    Guid HeartRateGuid = new Guid("0000180D-0000-1000-8000-00805F9B34FB");
                    Guid batterylevelGuid = new Guid("0000180F-0000-1000-8000-00805F9B34FB");
                    Guid ContinuuousO2guid = new Guid("46A970E0-0D5F-11E2-8B5E-0002A5D5C51B");

                    //  rootPage.NotifyUser(String.Format("Found {0} services", services.Count), NotifyType.StatusMessage);
                    foreach (var service in services)
                    {
                        //I only need some service here not all the service 
                        ServiceList.Items.Add(new ComboBoxItem { Content = DisplayHelpers.GetServiceName(service), Tag = service });

                        if (service.Uuid.Equals(HeartRateGuid))
                        {
                            getHeartRateServiceCharacter(service);
                        }
                       
                        if (service.Uuid.Equals(batterylevelGuid))
                        {
                            getBatteryLevelServiceCharacter(service);
                        }
                        
                        if (service.Uuid.Equals(ContinuuousO2guid))
                        {
                            getSPO2ServiceCharacter(service);
                        }

                    }
                    
                    ConnectButton.Visibility = Visibility.Collapsed;
                    ServiceList.Visibility = Visibility.Visible;
                }
                else
                {
                  //  rootPage.NotifyUser("Device unreachable", NotifyType.ErrorMessage);
                }
            }
            ConnectButton.IsEnabled = true;
        }

        #region SpO2 value get
       
        private async void getSPO2ServiceCharacter(GattDeviceService SPO2serviceTag)
        {
            var service = (GattDeviceService)SPO2serviceTag;
            // a list to stortage all the characteristic of spO2 service 
            IReadOnlyList<GattCharacteristic> spO2Characterist = null;
            //get all the spO2 characteristic 
            var spO2result = service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);

            try
            {
                var accessStatus = await service.RequestAccessAsync();
                if (accessStatus == DeviceAccessStatus.Allowed)
                {
                    var result = await service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);

                    if (result.Status == GattCommunicationStatus.Success)
                    {
                        spO2Characterist = result.Characteristics;
                    }
                    else
                    {
                        spO2Characterist = new List<GattCharacteristic>();
                    }
                }
                else
                {
                    spO2Characterist = new List<GattCharacteristic>();
                }
            }
            catch (Exception ex)
            {
                spO2Characterist = new List<GattCharacteristic>();
            }

            Guid Oimetrychracteristic = new Guid("0AAD7EA0-0D60-11E2-8E3C-0002A5D5C51B");
            Guid PITchracteristic = new Guid("34E27863-76FF-4F8E-96F1-9E3993AA6199");
            foreach (GattCharacteristic c in spO2Characterist)
            {
                
                if (c.Uuid.Equals(Oimetrychracteristic))
                {
                    SPO2CTag = c;
                };

                if (c.Uuid.Equals(PITchracteristic))
                {
                    PITCTag = c;
                }
            }
            getSpO2ValueDisplay();
            getPITValueDisply();
        }

        private async void getPITValueDisply()
        {
            GattCommunicationStatus status = GattCommunicationStatus.Unreachable;
            var cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;

            status = await PITCTag.WriteClientCharacteristicConfigurationDescriptorAsync(cccdValue);

            if (status == GattCommunicationStatus.Success)
            {
                PITCharacteristic = PITCTag;
                PITCharacteristic.ValueChanged += PITCharacteristic_ValueChanged;
            }
        }

        #region PIT from continous Oximetry 

        private async void PITCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var PITvalue = PITFormatValue(args.CharacteristicValue, presentationFormat);
            var PITmessage = PITvalue;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PITValue.Text = PITmessage);
        }

        private string PITFormatValue(IBuffer buffer, GattPresentationFormat format)
        {

            byte[] data;
            int pitMS = 0;
            string PIT1hex = "";
            string PIT2hex = "";


            CryptographicBuffer.CopyToByteArray(buffer, out data);

            if (data != null) { 
                var hexadecimal = BitConverter.ToString(data);
                if(hexadecimal != null){
                    try
                    {
                        PIT1hex = hexadecimal.Substring(18, 2);
                        PIT2hex = hexadecimal.Substring(21, 2);
                    }
                    catch(Exception e)
                    {
                        return "0";
                    }
                    
                    string PTI = "0x" + PIT1hex + PIT2hex;
                    var PITconvert = Convert.ToInt32(PTI, 16);
                    pitMS = Convert.ToInt32(PITconvert * 0.1);
                }
                
            }
           
            return pitMS.ToString() + "/ms";
        }

#endregion

        #region SpO2 from continous Oximetry 

        private async void getSpO2ValueDisplay()
        {
            GattCommunicationStatus status = GattCommunicationStatus.Unreachable;
            var cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;

            status = await SPO2CTag.WriteClientCharacteristicConfigurationDescriptorAsync(cccdValue);

            if (status == GattCommunicationStatus.Success)
            {
                spO2Characteristic = SPO2CTag;
                spO2Characteristic.ValueChanged += spO2Characteristic_ValueChanged;
            }
        }

        private async void spO2Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var SPO2formatvalue = SPO2FormatValue(args.CharacteristicValue, presentationFormat);
            var sp02message = SPO2formatvalue;
            SpO2Share = SPO2formatvalue;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => spO2Display.Text = sp02message);

            var PAIformatvalue = PAIFormatValue(args.CharacteristicValue, presentationFormat);
            var PAImessage = PAIformatvalue;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PAIDisplay.Text = PAImessage);
        }

        private string PAIFormatValue(IBuffer buffer, GattPresentationFormat presentationFormat)
        {
            byte[] data;
            decimal pai = 0;
            string PAI1hex = "";
            string PAI2hex = "";


            CryptographicBuffer.CopyToByteArray(buffer, out data);

            if (data != null)
            {
                var hexadecimal = BitConverter.ToString(data);
                if (hexadecimal != null)
                {
                    try
                    {
                        PAI1hex = hexadecimal.Substring(9, 2);
                        PAI2hex = hexadecimal.Substring(12, 2);
                    }
                    catch (Exception e)
                    {
                        return "0";
                    }

                    string PTI = "0x" + PAI1hex + PAI2hex;
                    var PITconvert = Convert.ToInt32(PTI, 16);
                    pai = Convert.ToDecimal(PITconvert*0.01);
                    
                }

            }

            return pai.ToString() + " %" ;
        }

        private string SPO2FormatValue(IBuffer buffer, GattPresentationFormat format)
        {

            byte[] data;
            CryptographicBuffer.CopyToByteArray(buffer, out data);
            var hexadecimal = BitConverter.ToString(data);
            string spo2hex = hexadecimal.Substring(21, 2);
            string spo2 = "0x" + spo2hex;
            var spo2convert = Convert.ToInt32(spo2, 16);

            return spo2convert.ToString();
        }

        #endregion


        //Pulse Amplitude Index 




        #endregion

        #region battery data retrive
        private async void getBatteryLevelServiceCharacter(GattDeviceService BLservice)
        {
            var BLVservice = (GattDeviceService)BLservice;
            IReadOnlyList<GattCharacteristic> BLVcharacteristics = null;
            var BLVresult = await BLservice.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
            try
            {
                var accessStatus = await BLVservice.RequestAccessAsync();
                if (accessStatus == DeviceAccessStatus.Allowed)
                {

                    var result = await BLVservice.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);

                    if (result.Status == GattCommunicationStatus.Success)
                    {
                        BLVcharacteristics = BLVresult.Characteristics;
                    }
                    else
                    {
                        BLVcharacteristics = new List<GattCharacteristic>();
                    }
                }
                else
                {
                    BLVcharacteristics = new List<GattCharacteristic>();
                }
            }
            catch (Exception ex)
            {
                BLVcharacteristics = new List<GattCharacteristic>();
            }

            //assight Battery Level Characteristic to BLCTag
            foreach (GattCharacteristic c in BLVcharacteristics)
            {
                if (DisplayHelpers.GetCharacteristicName(c).Equals("BatteryLevel"))
                {
                    BLCTag = c;
                };
            }
            getBatteryLevelValueDisplay();
        }

        private async void getBatteryLevelValueDisplay()
        {
            //only get value at once, need subscribe as well
            GattReadResult result = await BLCTag.ReadValueAsync(BluetoothCacheMode.Uncached);
            if (result.Status == GattCommunicationStatus.Success)
            {
                string formattedResult = BatteryLevelValueFormatValue(result.Value, presentationFormat);
                var message = formattedResult;

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => BatteryLevelData.Text = message);
            }

            //TODO: also need to subscribe
        }

        private void BatteryLevelChangedHandler()
        {
            BatteryCharacteristic = BLCTag;
            BLCTag.ValueChanged += BLVCharacteristic_ValueChangedAsync;
        }

        private async void BLVCharacteristic_ValueChangedAsync(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            
            var BatteryLevelValue = BatteryLevelValueFormatValue(args.CharacteristicValue, presentationFormat);
            var BLVmessage = BatteryLevelValue;
            
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => BatteryLevelData.Text = BLVmessage);
        }

        private string BatteryLevelValueFormatValue(IBuffer buffer, GattPresentationFormat format)
        {

            byte[] data;
            CryptographicBuffer.CopyToByteArray(buffer, out data);

            if (data != null)
            {
               
                if (BLCTag.Uuid.Equals(GattCharacteristicUuids.BatteryLevel))
                {
                    try
                    {
                        return "Battery Level: " + data[0].ToString() + "%";
                    }
                    catch (ArgumentException)
                    {
                        return "Battery Level: (unable to parse)";
                    }
                }

            }
            else
            {
                return "Empty data received";
            }
            return "Unknown format no data recived";
        }


        #endregion

        #region Heart Rate data retrive 

        //as soon as it get connect go straight to get the heart rate service and characteristic and assight it to  
        private async void getHeartRateServiceCharacter(GattDeviceService HRServiceTag)
        {
            var HRservice = (GattDeviceService)HRServiceTag;
            //storage all the Heart Rate characteristic 
            IReadOnlyList<GattCharacteristic> HRcharacteristics = null;
            //get all the Heart Rate characteristic
            var HRresult = await HRservice.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);

            //get the service and characteristic here
            try
            {
                var accessStatus = await HRservice.RequestAccessAsync();
                if (accessStatus == DeviceAccessStatus.Allowed)
                {
                   
                    var result = await HRservice.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);

                    if (result.Status == GattCommunicationStatus.Success)
                    {
                        HRcharacteristics = HRresult.Characteristics;
                    }
                    else
                    {
                        HRcharacteristics = new List<GattCharacteristic>();
                    }
                }
                else
                {
                    HRcharacteristics = new List<GattCharacteristic>();
                }
            }
            catch (Exception ex)
            {
                HRcharacteristics = new List<GattCharacteristic>();
            }

            //get the one characteristic that i need
            foreach (GattCharacteristic c in HRcharacteristics)
            {
                if (DisplayHelpers.GetCharacteristicName(c).Equals("HeartRateMeasurement"))
                {
                    HRCTag = c;
                };
            }
            getHeartRateValueDisplay();
        }

        private async void getHeartRateValueDisplay()
        {
            GattCommunicationStatus status = GattCommunicationStatus.Unreachable;
            var cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;
            
            try
            {
                status = await HRCTag.WriteClientCharacteristicConfigurationDescriptorAsync(cccdValue);

                if (status == GattCommunicationStatus.Success)
                {
                    HeartRateValueChangedHandler();
                }
               
            }
            catch (UnauthorizedAccessException ex)
            {
               
            }
        }

        private void HeartRateValueChangedHandler()
        {
             HartRateCharacteristic = HRCTag;
             HRCTag.ValueChanged += HRCharacteristic_ValueChanged;
        }


        private async void HRCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
          
            var HeartRatevalue = HeartRateFormatValue(args.CharacteristicValue, presentationFormat);
            var HRmessage = HeartRatevalue;
            heartrateShare = HeartRatevalue;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HeartReteDataDisply.Text = HeartRatevalue);
        }

        private string HeartRateFormatValue(IBuffer buffer, GattPresentationFormat format)
        {
           
            byte[] data;
            CryptographicBuffer.CopyToByteArray(buffer, out data);
           
            if (data != null)
            {

                if (HRCTag.Uuid.Equals(GattCharacteristicUuids.HeartRateMeasurement))
                {
                    try
                    {
                        return ParseHeartRateValue(data).ToString();
                    }
                    catch (ArgumentException)
                    {
                        return "Heart Rate: (unable to parse)";
                    }
                }
                
            }
            else
            {
                return "Empty data received";
            }
            return "Unknown format no data recived";
        }

        private static ushort ParseHeartRateValue(byte[] data)
        {
            // Heart Rate profile defined flag values
            const byte heartRateValueFormat = 0x01;

            byte flags = data[0];
            bool isHeartRateValueSizeLong = ((flags & heartRateValueFormat) != 0);

            if (isHeartRateValueSizeLong)
            {
                return BitConverter.ToUInt16(data, 1);
            }
            else
            {
                return data[1];
            }
        }



        #endregion 


        //choosed servie and find the characteristic in the service  
        private async void ServiceList_SelectionChanged()
        {
            var service = (GattDeviceService)((ComboBoxItem)ServiceList.SelectedItem)?.Tag;
            CharacteristicList.Items.Clear();
            RemoveValueChangedHandler();

            IReadOnlyList<GattCharacteristic> characteristics = null;
            try
            {
                // Ensure we have access to the device.
                var accessStatus = await service.RequestAccessAsync();
                if (accessStatus == DeviceAccessStatus.Allowed)
                {
                    // BT_Code: Get all the child characteristics of a service. Use the cache mode to specify uncached characterstics only 
                    // and the new Async functions to get the characteristics of unpaired devices as well. 
                    var result = await service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
                   
                    if (result.Status == GattCommunicationStatus.Success)
                    {
                        characteristics = result.Characteristics;  
                    }
                    else
                    {
                       // rootPage.NotifyUser("Error accessing service.", NotifyType.ErrorMessage);

                        // On error, act as if there are no characteristics.
                        characteristics = new List<GattCharacteristic>();
                    }
                }
                else
                {
                    // Not granted access
                  //  rootPage.NotifyUser("Error accessing service.", NotifyType.ErrorMessage);

                    // On error, act as if there are no characteristics.
                    characteristics = new List<GattCharacteristic>();

                }
            }
            catch (Exception ex)
            {
              //  rootPage.NotifyUser("Restricted service. Can't read characteristics: " + ex.Message,
                 //   NotifyType.ErrorMessage);
                // On error, act as if there are no characteristics.
                characteristics = new List<GattCharacteristic>();
            }

            foreach (GattCharacteristic c in characteristics)
            {
                CharacteristicList.Items.Add(new ComboBoxItem { Content = c.Uuid, Tag = c });
            }
            CharacteristicList.Visibility = Visibility.Visible;
        }




        //remove the characteristic that is already excited so that can be able to add new in
        private void RemoveValueChangedHandler()
        {
            ValueChangedSubscribeToggle.Content = "Subscribe to value changes";
            if (subscribedForNotifications)
            {
                registeredCharacteristic.ValueChanged -= Characteristic_ValueChanged;
                registeredCharacteristic = null;
                subscribedForNotifications = false;
            }
        }

        private async void CharacteristicList_SelectionChanged()
        {
            selectedCharacteristic = (GattCharacteristic)((ComboBoxItem)CharacteristicList.SelectedItem)?.Tag;
            
            
            if (selectedCharacteristic == null)
            {
                EnableCharacteristicPanels(GattCharacteristicProperties.None);
                //rootPage.NotifyUser("No characteristic selected", NotifyType.ErrorMessage);
                return;
            }

            // Get all the child descriptors of a characteristics. Use the cache mode to specify uncached descriptors only 
            // and the new Async functions to get the descriptors of unpaired devices as well. 
            var result = await selectedCharacteristic.GetDescriptorsAsync(BluetoothCacheMode.Uncached);
           
            if (result.Status != GattCommunicationStatus.Success)
            {
                
                // rootPage.NotifyUser("Descriptor read failure: " + result.Status.ToString(), NotifyType.ErrorMessage);
            }

            // BT_Code: There's no need to access presentation format unless there's at least one. 
            presentationFormat = null;
            if (selectedCharacteristic.PresentationFormats.Count > 0)
            {

                if (selectedCharacteristic.PresentationFormats.Count.Equals(1))
                {
                    // Get the presentation format since there's only one way of presenting it
                    presentationFormat = selectedCharacteristic.PresentationFormats[0];
                  
                }
                else
                {
                    // It's difficult to figure out how to split up a characteristic and encode its different parts properly.
                    // In this case, we'll just encode the whole thing to a string to make it easy to print out.
                }
            }
            // Enable/disable operations based on the GattCharacteristicProperties.
            EnableCharacteristicPanels(selectedCharacteristic.CharacteristicProperties);
        }

        private void EnableCharacteristicPanels(GattCharacteristicProperties properties)
        {
            // BT_Code: Hide the controls which do not apply to this characteristic.
            SetVisibility(CharacteristicReadButton, properties.HasFlag(GattCharacteristicProperties.Read));


            SetVisibility(ValueChangedSubscribeToggle, properties.HasFlag(GattCharacteristicProperties.Indicate) ||
                                                       properties.HasFlag(GattCharacteristicProperties.Notify));

        }

        private void SetVisibility(UIElement element, bool visible)
        {
            element.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// after finding the characher after here are reading the value  and formating 
        /// </summary>
        private async void CharacteristicReadButton_Click()
        {
            // BT_Code: Read the actual value from the device by using Uncached.
            GattReadResult result = await selectedCharacteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
            if (result.Status == GattCommunicationStatus.Success)
            {
                string formattedResult = FormatValueByPresentation(result.Value, presentationFormat);
                //rootPage.NotifyUser($"Read result: {formattedResult}", NotifyType.StatusMessage);
                
            }
            else
            {
                //rootPage.NotifyUser($"Read failed: {result.Status}", NotifyType.ErrorMessage);
            }
        }

        private bool subscribedForNotifications = false;
        private async void ValueChangedSubscribeToggle_Click()
        {
            if (!subscribedForNotifications)
            {
                // initialize status
                GattCommunicationStatus status = GattCommunicationStatus.Unreachable;
                var cccdValue = GattClientCharacteristicConfigurationDescriptorValue.None;
                if (selectedCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
                {
                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
                }

                else if (selectedCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                {
                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;
                }

                try
                {
                    // BT_Code: Must write the CCCD in order for server to send indications.
                    // We receive them in the ValueChanged event handler.
                    status = await selectedCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(cccdValue);

                    if (status == GattCommunicationStatus.Success)
                    {
                        AddValueChangedHandler();
                       // rootPage.NotifyUser("Successfully subscribed for value changes", NotifyType.StatusMessage);
                    }
                    else
                    {
                        //rootPage.NotifyUser($"Error registering for value changes: {status}", NotifyType.ErrorMessage);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    // This usually happens when a device reports that it support indicate, but it actually doesn't.
                    //rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
                }
            }
            else
            {
                try
                {
                    // BT_Code: Must write the CCCD in order for server to send notifications.
                    // We receive them in the ValueChanged event handler.
                    // Note that this sample configures either Indicate or Notify, but not both.
                    var result = await selectedCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                    if (result == GattCommunicationStatus.Success)
                    {
                        subscribedForNotifications = false;
                        RemoveValueChangedHandler();
                       // rootPage.NotifyUser("Successfully un-registered for notifications", NotifyType.StatusMessage);
                    }
                    else
                    {
                       // rootPage.NotifyUser($"Error un-registering for notifications: {result}", NotifyType.ErrorMessage);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    // This usually happens when a device reports that it support notify, but it actually doesn't.
                   // rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
                }
            }
        }

        

        private void AddValueChangedHandler()
        {
            ValueChangedSubscribeToggle.Content = "Unsubscribe from value changes";
            if (!subscribedForNotifications)
            {
                registeredCharacteristic = selectedCharacteristic;
                registeredCharacteristic.ValueChanged += Characteristic_ValueChanged;
                subscribedForNotifications = true;

            }
        }

        private async void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            // BT_Code: An Indicate or Notify reported that the value has changed.
            // Display the new value with a timestamp.
            var newValue = FormatValueByPresentation(args.CharacteristicValue, presentationFormat);

            var message =  newValue;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => CharacteristicLatestValue.Text = message);
        }



        private string FormatValueByPresentation(IBuffer buffer, GattPresentationFormat format)
        {
            // BT_Code: For the purpose of this sample, this function converts only UInt32 and
            // UTF-8 buffers to readable text. It can be extended to support other formats if your app needs them.
            byte[] data;
            CryptographicBuffer.CopyToByteArray(buffer, out data);
            if (format != null)
            {
                if (format.FormatType == GattPresentationFormatTypes.UInt32 && data.Length >= 4)
                {
                    return BitConverter.ToInt32(data, 0).ToString();
                }
                else if (format.FormatType == GattPresentationFormatTypes.Utf8)
                {
                    try
                    {
                        return Encoding.UTF8.GetString(data);
                    }
                    catch (ArgumentException)
                    {
                        return "(error: Invalid UTF-8 string)";
                    }
                }
                else if (format.FormatType == GattPresentationFormatTypes.UInt16)
                {
                    try
                    {
                        return BitConverter.ToInt16(data,0).ToString();
                    }
                    catch (ArgumentException)
                    {
                        return "(error: Invalid UTF-16 string)";
                    }
                }
                else
                {
                    // Add support for other format types as needed.
                    return "Unsupported format: " + CryptographicBuffer.EncodeToHexString(buffer);
                }
            }
            else if (data != null)
            {
                // We don't know what format to use. Let's try some well-known profiles, or default back to UTF-8.
                if (selectedCharacteristic.Uuid.Equals(GattCharacteristicUuids.HeartRateMeasurement))
                {
                    try
                    {

                        return ParseHeartRateValue(data).ToString();
                    }
                    catch (ArgumentException)
                    {
                        return "Heart Rate: (unable to parse)";
                    }
                }
                else if (selectedCharacteristic.Uuid.Equals(GattCharacteristicUuids.BatteryLevel))
                {
                    try
                    {
                        // battery level is encoded as a percentage value in the first byte according to
                        // https://www.bluetooth.com/specifications/gatt/viewer?attributeXmlFile=org.bluetooth.characteristic.battery_level.xml
                        return "Battery Level: " + data[0].ToString() + "%";
                    }
                    catch (ArgumentException)
                    {
                        return "Battery Level: (unable to parse)";
                    }
                }
                // This is our custom calc service Result UUID. Format it like an Int
                else if (selectedCharacteristic.Uuid.Equals(Constants.ResultCharacteristicUuid))
                {
                    return BitConverter.ToInt32(data, 0).ToString();
                }
                // No guarantees on if a characteristic is registered for notifications.
                else if (registeredCharacteristic != null)
                {
                    // This is our custom calc service Result UUID. Format it like an Int
                    if (registeredCharacteristic.Uuid.Equals(Constants.ResultCharacteristicUuid))
                    {
                        return BitConverter.ToInt32(data, 0).ToString();
                    }
                }
                else
                {
                    try
                    {
                        return "Unknown format: " + Encoding.UTF8.GetString(data);
                    }
                    catch (ArgumentException)
                    {
                        return "Unknown format uuid" + registeredCharacteristic.Uuid ;
                    }
                }
            }
            else
            {
                return "Empty data received";
            }
            return BitConverter.ToString(data);
        }

        
    }
}
