uWebshop-Releases
=================

The nuget package is created by building the buildproject proj.
This creates a .nupkg inside the .nuget folder in the root solution directory.
To increment the version number edit uWebshop.Umbraco/Installer/package.nuspec

version 2.7.0.0
 - Fixed Category Cache issue
 - Removed (most) Umbraco Legacy API calls from the codebase (not on v6 datatypes)

version 2.6.5.0
 - Added 'CancelUrl' to payment provider API
 - Removed order section nodes, orders are now displayed on the orders node itself in a listview
 - Cleaned up some messy source
 - Some small fixes
 
Version 2.6.0.0
- Bugfix for Global Vat & Vat Picker on Umbraco v7
- Removed all licensing checks
- Various other bugfixes and small improvements
