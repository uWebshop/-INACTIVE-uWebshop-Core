uWebshop-Releases
=================

The uWebshop Package build is created in the /builds/Umbraco Packages/ folder.
One for Umbraco 6.1+, one for Umbraco 7.1+
Creating a zip file of the folder will give you an Umbraco Package to install!
Minor versions can be updated by simply coping over the DLL files to your /bin/ folder.

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