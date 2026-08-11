#!/usr/bin/env python3
"""Build spec-derived .packet fixtures for packets that have no real game capture.

Run from the repository root: python3 docs/concepts/generate_2026_fixtures.py

Headers are copied byte-for-byte from an existing real sample of the same game
year and only the packet id (offset 6) is patched, so the header stays exactly
as the game emits it. Payloads are assembled from the byte layouts documented in
docs/F1 2026 Telemetry.md (and the 2025 equivalent), with a distinctive value per
field so a test assertion pins down the offset it belongs to.
"""
import struct
import pathlib

SAMPLES = pathlib.Path("F1Server.Tests/SampleData")
HEADER_SIZE = 29
PACKET_ID_OFFSET = 6

# Raw wire ids: PacketTypes enum value minus one (ReceivedPacketData does packetType + 1)
RAW_ID_TIME_TRIAL = 14
RAW_ID_LAP_POSITIONS = 15
RAW_ID_CAR_TELEMETRY2 = 16


def header_from(sample_name, raw_packet_id):
    """Take the 29 byte header of a real capture and patch in the wanted packet id."""
    raw = bytearray((SAMPLES / sample_name).read_bytes()[:HEADER_SIZE])
    assert len(raw) == HEADER_SIZE, f"{sample_name} shorter than a header"
    raw[PACKET_ID_OFFSET] = raw_packet_id
    return bytes(raw)


def time_trial_set(car_idx, team_id, lap_ms, s1, s2, s3, tc, gearbox, abs_on, equal_perf, custom, valid, team_uint16):
    """One TimeTrialDataSet. The team id is uint8 up to F1 2025 and uint16 from F1 2026."""
    out = struct.pack("<B", car_idx)
    out += struct.pack("<H", team_id) if team_uint16 else struct.pack("<B", team_id)
    out += struct.pack("<IIII", lap_ms, s1, s2, s3)
    out += struct.pack("<BBBBBB", tc, gearbox, abs_on, equal_perf, custom, valid)
    return out


def build_time_trial(year, team_uint16):
    """PacketTimeTrialData: player session best, personal best, rival."""
    # tractionControl 1 = Medium, gearboxAssist 1 = Manual (0 maps to Unknown, which asserts nothing)
    body = time_trial_set(19, 9, 91234, 30111, 31222, 29901, 1, 1, 1, 0, 1, 1, team_uint16)
    body += time_trial_set(19, 9, 90567, 29876, 31001, 29690, 0, 1, 0, 1, 0, 1, team_uint16)
    body += time_trial_set(7, 42 if team_uint16 else 4, 89999, 29500, 30800, 29699, 1, 1, 1, 0, 0, 1, team_uint16)
    expected = 75 if team_uint16 else 72
    assert len(body) == expected, f"time trial {year}: {len(body)} != {expected}"
    return body


def build_lap_positions():
    """PacketLapPositionsData: numLaps, lapStart, then position[50 laps][24 cars]."""
    num_laps, lap_start, max_laps, max_cars = 5, 0, 50, 24
    body = struct.pack("<BB", num_laps, lap_start)
    grid = bytearray(max_laps * max_cars)
    for lap in range(num_laps):
        for car in range(max_cars):
            # Distinctive but stable: car 0 leads on lap 0, the order rotates by one each lap
            grid[(lap * max_cars) + car] = ((car + lap) % max_cars) + 1
    body += bytes(grid)
    assert len(body) == 2 + (max_laps * max_cars) == 1202, f"lap positions: {len(body)}"
    return body


def build_car_telemetry2():
    """PacketCarTelemetry2Data: CarTelemetry2Data[24], 10 bytes each."""
    body = b""
    for car in range(24):
        body += struct.pack(
            "<BBHBBHBB",
            car % 2,                 # activeAeroMode: 0 corner, 1 straight
            1 if car < 20 else 0,    # activeAeroAvailable
            (car * 10) % 1000,       # activeAeroActivationDistance
            1 if car % 3 == 0 else 0,  # overtakeAvailable
            1 if car == 19 else 0,   # overtakeActive
            (car * 25) % 2000,       # overtakeActivationDistance
            1,                       # 2026 regulations apply
            1 if car == 5 else 0,    # drivingWrongWay
        )
    assert len(body) == 240, f"car telemetry2: {len(body)}"
    return body


def write(name, header, body):
    path = SAMPLES / name
    path.write_bytes(header + body)
    print(f"{name}: {len(header) + len(body)} bytes ({len(body)} payload)")


write("F1-2025-TimeTrial.packet",
      header_from("F1-2025-LapData.packet", RAW_ID_TIME_TRIAL),
      build_time_trial(2025, team_uint16=False))

write("F1-2026-TimeTrial.packet",
      header_from("F1-2026-LapData.packet", RAW_ID_TIME_TRIAL),
      build_time_trial(2026, team_uint16=True))

write("F1-2026-LapPositions.packet",
      header_from("F1-2026-LapData.packet", RAW_ID_LAP_POSITIONS),
      build_lap_positions())

write("F1-2026-CarTelemetry2.packet",
      header_from("F1-2026-LapData.packet", RAW_ID_CAR_TELEMETRY2),
      build_car_telemetry2())
