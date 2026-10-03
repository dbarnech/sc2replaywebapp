import sys
import mpyq

archive = mpyq.MPQArchive(sys.argv[1])

metadata = archive.read_file("replay.gamemetadata.json")

print(metadata.decode("utf-8"))

details = archive.read_file("replay.details")

print(details.hex()[:100])