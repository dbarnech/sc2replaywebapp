from datetime import datetime, timedelta
import json
import sys
import pprint
import mpyq
from s2protocol import versions

archive = mpyq.MPQArchive(sys.argv[1])

metadata = archive.read_file("replay.gamemetadata.json")

game_data = json.loads(metadata.decode("utf-8"))

build = int(game_data["DataBuild"])

try:
    protocol = versions.build(build)
except KeyError:
    raise Exception(
        f"Unsupported SC2  replay build: {build}. Upgrade S2protocol."
    )

details_bytes = archive.read_file("replay.details")
details = protocol.decode_replay_details(details_bytes)

filetime = details["m_timeUTC"]
detail_players = details["m_playerList"]

game_date = (datetime(1601, 1, 1) + timedelta(microseconds=filetime / 10)).isoformat() + "Z"

duration_seconds = int(game_data["Duration"] / 1.4)

winner = "Unknown"

players = []

for index, player in enumerate(game_data["Players"]):

    detail_player = detail_players[index]
    name = detail_player["m_name"].decode("utf-8")
    race = detail_player["m_race"].decode("utf-8")
    won = detail_player["m_result"] == 1

    if won:
        winner = name

    players.append({
        "PlayerID": int(player['PlayerID']),
        "Name": name,
        "APM": int(player["APM"]),
        "Result": player["Result"],
        "Race": race,
        "Won": won
    })

result = {
    "Title": game_data["Title"],
    "GameVersion": game_data["GameVersion"],
    "DataBuild": game_data["DataBuild"],
    "DataVersion": game_data["DataVersion"],
    "BaseBuild": game_data["BaseBuild"],
    "Duration": int(game_data["Duration"]),
    "GameDate": game_date,
    "DetailsHex": details_bytes.hex(),
    "DetailsRaw": str(details),
    "Winner": winner,
    "DurationSeconds": duration_seconds,
    "Players": players
}

print(json.dumps(result))

