# SC2 Replay Web App


## Update Python S2Protocol to the latest version
If Blizzard updates the SC2 protocol, you may need to update the S2Protocol Python package to the latest version. You can do this by running the following command:
```bash
py -3.11 -m pip install --upgrade s2protocol
```

Verify installation by running:
```bash
py -3.11 -m pip show s2protocol
``
```

Expected output:
```bash
Name: s2protocol
Version: 5.0.17.98310.0
Summary: Python library to decode StarCraft II replay protocols
Home-page: https://github.com/Blizzard/s2protocol
Author: Blizzard Entertainment
Author-email: s2github@blizzard.com
License:
...
```
