using System;
using System.Collections.Generic;
using System.Linq;

namespace SZ2.ECUSimulatorGUI.Service.OBD2
{
    public class OBD2ContentTable
    {
        private readonly Dictionary<OBD2ParameterCode, OBD2NumericContent> _numeric_content_table = new Dictionary<OBD2ParameterCode, OBD2NumericContent>();
        private readonly Dictionary<byte, UInt32> _availablePIDFlagMap = new Dictionary<byte, UInt32>();
        public OBD2ContentTable()
        {
            setNumericContentTable();
        }

        public Dictionary<OBD2ParameterCode, OBD2NumericContent> Table { get => _numeric_content_table; }
        
        private void setNumericContentTable()
        {
            _numeric_content_table.Add(OBD2ParameterCode.Engine_Load, new OBD2NumericContent(0x04, 1, A => A * 100 / 255, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Coolant_Temperature, new OBD2NumericContent(0x05, 1, A => A - 40, "degC"));
            _numeric_content_table.Add(OBD2ParameterCode.Air_Fuel_Correction_1, new OBD2NumericContent(0x06, 1, A => (A - 128) * 100 / 128, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Air_Fuel_Learning_1, new OBD2NumericContent(0x07, 1, A => (A - 128) * 100 / 128, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Air_Fuel_Correction_2, new OBD2NumericContent(0x08, 1, A => (A - 128) * 100 / 128, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Air_Fuel_Learning_2, new OBD2NumericContent(0x09, 1, A => (A - 128) * 100 / 128, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Fuel_Tank_Pressure, new OBD2NumericContent(0x0A, 1, A => A * 3, "kPa"));
            _numeric_content_table.Add(OBD2ParameterCode.Manifold_Absolute_Pressure, new OBD2NumericContent(0x0B, 1, A => A, "kPa"));
            _numeric_content_table.Add(OBD2ParameterCode.Engine_Speed, new OBD2NumericContent(0x0C, 2, A => A / 4, "rpm"));
            _numeric_content_table.Add(OBD2ParameterCode.Vehicle_Speed, new OBD2NumericContent(0x0D, 1, A => A, "km/h"));
            _numeric_content_table.Add(OBD2ParameterCode.Ignition_Timing, new OBD2NumericContent(0x0E, 1, A => A / 2 - 64, "deg"));
            _numeric_content_table.Add(OBD2ParameterCode.Intake_Air_Temperature, new OBD2NumericContent(0x0F, 1, A => A - 40, "degC"));
            _numeric_content_table.Add(OBD2ParameterCode.Mass_Air_Flow, new OBD2NumericContent(0x10, 2, A => A / 100, "g/s"));
            _numeric_content_table.Add(OBD2ParameterCode.Throttle_Opening_Angle, new OBD2NumericContent(0x11, 1, A => A * 100 / 255, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Run_time_since_engine_start, new OBD2NumericContent(0x1F, 2, A => A, "seconds"));
            _numeric_content_table.Add(OBD2ParameterCode.Distance_traveled_with_MIL_on, new OBD2NumericContent(0x21, 2, A => A, "km"));
            _numeric_content_table.Add(OBD2ParameterCode.Fuel_Rail_Pressure, new OBD2NumericContent(0x22, 2, A => (A * 10) / 128, "kPa"));
            _numeric_content_table.Add(OBD2ParameterCode.Fuel_Rail_Pressure_diesel, new OBD2NumericContent(0x23, 2, A => A * 10, "kPa"));
            _numeric_content_table.Add(OBD2ParameterCode.Commanded_EGR, new OBD2NumericContent(0x2C, 1, A => 100 * A / 255, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.EGR_Error, new OBD2NumericContent(0x2D, 1, A => (A - 128) * 100 / 128, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Commanded_evaporative_purge, new OBD2NumericContent(0x2E, 1, A => 100 * A / 255, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Fuel_Level_Input, new OBD2NumericContent(0x2F, 1, A => 100 * A / 255, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Number_of_warmups_since_codes_cleared, new OBD2NumericContent(0x30, 1, A => A, "N/A"));
            _numeric_content_table.Add(OBD2ParameterCode.Distance_traveled_since_codes_cleared, new OBD2NumericContent(0x31, 2, A => A, "km"));
            _numeric_content_table.Add(OBD2ParameterCode.Evap_System_Vapor_Pressure, new OBD2NumericContent(0x32, 2, A => A / 4, "Pa"));
            _numeric_content_table.Add(OBD2ParameterCode.Atmospheric_Pressure, new OBD2NumericContent(0x33, 1, A => A, "kPa "));
            _numeric_content_table.Add(OBD2ParameterCode.Catalyst_TemperatureBank_1_Sensor_1, new OBD2NumericContent(0x3C, 2, A => A / 10 - 40, "degC"));
            _numeric_content_table.Add(OBD2ParameterCode.Catalyst_TemperatureBank_2_Sensor_1, new OBD2NumericContent(0x3D, 2, A => A / 10 - 40, "degC"));
            _numeric_content_table.Add(OBD2ParameterCode.Catalyst_TemperatureBank_1_Sensor_2, new OBD2NumericContent(0x3E, 2, A => A / 10 - 40, "degC"));
            _numeric_content_table.Add(OBD2ParameterCode.Catalyst_TemperatureBank_2_Sensor_2, new OBD2NumericContent(0x3F, 2, A => A / 10 - 40, "degC"));
            _numeric_content_table.Add(OBD2ParameterCode.Battery_Voltage, new OBD2NumericContent(0x42, 2, A => A / 1000, "V"));
            _numeric_content_table.Add(OBD2ParameterCode.Absolute_load_value, new OBD2NumericContent(0x43, 2, A => A * 100 / 255, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Command_equivalence_ratio, new OBD2NumericContent(0x44, 2, A => A / 32768, "N/A"));
            _numeric_content_table.Add(OBD2ParameterCode.Relative_throttle_position, new OBD2NumericContent(0x45, 1, A => A * 100 / 255, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Ambient_air_temperature, new OBD2NumericContent(0x46, 1, A => A - 40, "degC"));
            _numeric_content_table.Add(OBD2ParameterCode.Absolute_throttle_position_B, new OBD2NumericContent(0x47, 1, A => A * 100 / 255, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Absolute_throttle_position_C, new OBD2NumericContent(0x48, 1, A => A * 100 / 255, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Accelerator_pedal_position_D, new OBD2NumericContent(0x49, 1, A => A * 100 / 255, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Accelerator_pedal_position_E, new OBD2NumericContent(0x4A, 1, A => A * 100 / 255, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Accelerator_pedal_position_F, new OBD2NumericContent(0x4B, 1, A => A * 100 / 255, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Commanded_throttle_actuator, new OBD2NumericContent(0x4C, 1, A => A * 100 / 255, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Time_run_with_MIL_on, new OBD2NumericContent(0x4D, 2, A => A, "minutes"));
            _numeric_content_table.Add(OBD2ParameterCode.Time_since_trouble_codes_cleared, new OBD2NumericContent(0x4E, 2, A => A, "minutes"));
            _numeric_content_table.Add(OBD2ParameterCode.Ethanol_fuel_percent, new OBD2NumericContent(0x52, 1, A => A * 100 / 255, "%"));

            // Added on 2018/01/07
            _numeric_content_table.Add(OBD2ParameterCode.Evap_system_vapor_pressure, new OBD2NumericContent(0x54, 2, A => A - 32767, "Pa"));
            _numeric_content_table.Add(OBD2ParameterCode.Fuel_rail_absolute_pressure, new OBD2NumericContent(0x59, 2, A => 10 * A, "kPa"));
            _numeric_content_table.Add(OBD2ParameterCode.Relative_accelerator_pedal_position, new OBD2NumericContent(0x5A, 1, A => 100 * A / 255, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Hybrid_battery_pack_remaining_life, new OBD2NumericContent(0x5B, 1, A => 100 * A / 255, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Engine_oil_temperature, new OBD2NumericContent(0x5C, 1, A => A - 40, "degC"));
            _numeric_content_table.Add(OBD2ParameterCode.Fuel_injection_timing, new OBD2NumericContent(0x5D, 2, A => A / 128 - 210, "degC"));
            _numeric_content_table.Add(OBD2ParameterCode.Engine_fuel_rate, new OBD2NumericContent(0x5E, 2, A => A / 20, "L/h"));
            _numeric_content_table.Add(OBD2ParameterCode.Driver_demand_engine_percent_torque, new OBD2NumericContent(0x61, 1, A => A - 125, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Actual_engine_percent_torque, new OBD2NumericContent(0x62, 1, A => A - 125, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.Engine_reference_torque, new OBD2NumericContent(0x63, 2, A => A, "Nm"));

            _numeric_content_table.Add(OBD2ParameterCode.O2Sensor_1_Air_Fuel_Correction, new OBD2NumericContent(0x14, 2, A => ((double)((int)A & 0xFF)) / 128 - 100, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.O2Sensor_2_Air_Fuel_Correction, new OBD2NumericContent(0x15, 2, A => ((double)((int)A & 0xFF)) / 128 - 100, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.O2Sensor_3_Air_Fuel_Correction, new OBD2NumericContent(0x16, 2, A => ((double)((int)A & 0xFF)) / 128 - 100, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.O2Sensor_4_Air_Fuel_Correction, new OBD2NumericContent(0x17, 2, A => ((double)((int)A & 0xFF)) / 128 - 100, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.O2Sensor_5_Air_Fuel_Correction, new OBD2NumericContent(0x18, 2, A => ((double)((int)A & 0xFF)) / 128 - 100, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.O2Sensor_6_Air_Fuel_Correction, new OBD2NumericContent(0x19, 2, A => ((double)((int)A & 0xFF)) / 128 - 100, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.O2Sensor_7_Air_Fuel_Correction, new OBD2NumericContent(0x1A, 2, A => ((double)((int)A & 0xFF)) / 128 - 100, "%"));
            _numeric_content_table.Add(OBD2ParameterCode.O2Sensor_8_Air_Fuel_Correction, new OBD2NumericContent(0x1B, 2, A => ((double)((int)A & 0xFF)) / 128 - 100, "%"));

            _numeric_content_table.Add(OBD2ParameterCode.O2Sensor_1_Air_Fuel_Ratio, new OBD2NumericContent(0x24, 4, A => ((double)((int)A >> 16)) / 65536 * 2, "Lambda"));
            _numeric_content_table.Add(OBD2ParameterCode.O2Sensor_2_Air_Fuel_Ratio, new OBD2NumericContent(0x25, 4, A => ((double)((int)A >> 16)) / 65536 * 2, "Lambda"));
            _numeric_content_table.Add(OBD2ParameterCode.O2Sensor_3_Air_Fuel_Ratio, new OBD2NumericContent(0x26, 4, A => ((double)((int)A >> 16)) / 65536 * 2, "Lambda"));
            _numeric_content_table.Add(OBD2ParameterCode.O2Sensor_4_Air_Fuel_Ratio, new OBD2NumericContent(0x27, 4, A => ((double)((int)A >> 16)) / 65536 * 2, "Lambda"));
            _numeric_content_table.Add(OBD2ParameterCode.O2Sensor_5_Air_Fuel_Ratio, new OBD2NumericContent(0x28, 4, A => ((double)((int)A >> 16)) / 65536 * 2, "Lambda"));
            _numeric_content_table.Add(OBD2ParameterCode.O2Sensor_6_Air_Fuel_Ratio, new OBD2NumericContent(0x29, 4, A => ((double)((int)A >> 16)) / 65536 * 2, "Lambda"));
            _numeric_content_table.Add(OBD2ParameterCode.O2Sensor_7_Air_Fuel_Ratio, new OBD2NumericContent(0x2A, 4, A => ((double)((int)A >> 16)) / 65536 * 2, "Lambda"));
            _numeric_content_table.Add(OBD2ParameterCode.O2Sensor_8_Air_Fuel_Ratio, new OBD2NumericContent(0x2B, 4, A => ((double)((int)A >> 16)) / 65536 * 2, "Lambda"));
        }
    }
}