//import 'dart:convert';
//import 'dart:typed_data';
import 'dart:async';
//import 'package:async/async.dart';
import 'package:file_picker/file_picker.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import '../logic/hasher.dart';
import 'dart:io';

class AppUi extends StatefulWidget {
  const AppUi({Key? key}) : super(key: key);

  @override
  _AppUiState createState() => _AppUiState();
}

class _AppUiState extends State<AppUi> {
  String hashCodeGenerated = " ";
  String hashFilename = "";
  String fileToCountHashFilename = "";
  final inputController = TextEditingController(text: "");
  @override
  Widget build(BuildContext context) {
    return Container(
        child: Column(
      children: [
        Row(
          children: [
            Expanded(
              flex: 1,
              child: Padding(
                padding: EdgeInsets.symmetric(horizontal: 5, vertical: 10),
                child: TextField(
                  controller: inputController,
                  keyboardType: TextInputType.number,
                  decoration: InputDecoration(border: OutlineInputBorder(), labelText: "input data"),
                ),
              ),
            ),
            Expanded(
              flex: 1,
              child: Padding(
                padding: EdgeInsets.symmetric(vertical: 0, horizontal: 50),
                child: ElevatedButton(
                  child: Text("Generate hash"),
                  onPressed: () => {_generateHash(inputController.text)},
                ),
              ),
            ),
          ],
        ),
        Row(
          children: [
            Expanded(
              flex: 1,
              child: Padding(
                padding: EdgeInsets.symmetric(horizontal: 50),
                child: ElevatedButton(
                  child: Text("Pick file and generate hash"),
                  onPressed: () => {
                    _generateHashFromFile(),
                  },
                ),
              ),
            ),
          ],
        ),
        Row(
          children: [
            Expanded(
              flex: 2,
              child: Padding(
                padding: EdgeInsets.symmetric(horizontal: 50),
                child: Text("Filename: $hashFilename"),
              ),
            ),
            Expanded(
              flex: 1,
              child: Padding(
                padding: EdgeInsets.symmetric(horizontal: 50, vertical: 10),
                child: ElevatedButton(
                  child: Text("Pick file where hash is stored"),
                  onPressed: () => {
                    _pickMD5HashFile(),
                  },
                ),
              ),
            ),
          ],
        ),
        Row(
          children: [
            Expanded(
              flex: 2,
              child: Padding(
                padding: EdgeInsets.symmetric(horizontal: 50),
                child: Text("Filename: $fileToCountHashFilename"),
              ),
            ),
            Expanded(
              flex: 1,
              child: Padding(
                padding: EdgeInsets.symmetric(horizontal: 50, vertical: 10),
                child: ElevatedButton(
                  child: Text("Pick file to count hash"),
                  onPressed: () => {
                    _pickMD5FileToHash(),
                  },
                ),
              ),
            ),
          ],
        ),
        Row(
          children: [
            Expanded(
              flex: 1,
              child: Padding(
                padding: EdgeInsets.symmetric(horizontal: 50),
                child: ElevatedButton(
                  child: Text("Generate hash and compare"),
                  onPressed: () => {
                    _generateHashAndCompare(),
                  },
                ),
              ),
            ),
          ],
        ),
        Text("Result: "),
        Padding(
          padding: EdgeInsets.all(20),
          child: Container(
            width: double.infinity,
            height: 300,
            decoration: BoxDecoration(color: Colors.white, borderRadius: BorderRadius.all(Radius.circular(5)), border: Border.all(width: 1, color: Colors.black)),
            child: Text(hashCodeGenerated),
          ),
        ),
      ],
    ));
  }

  void _generateHash(String text) {
    var hasher = new MD5Hasher();
    var hash = hasher.generateMD5Hash(text);
    setState(() {
      // var data = utf8.encode(text);

      // var result = hasher.generatehash(Uint8List.fromList(data));
      // hashCodeGenerated = _hexEncode(result);
      hashCodeGenerated = hash;
    });
  }

  void _pickMD5HashFile() async {
    FilePickerResult? result = await FilePicker.platform.pickFiles(type: FileType.custom, allowedExtensions: ['md5'], dialogTitle: "Select your md5 file");
    if (result != null) {
      String path = result.files.single.path;
      setState(() {
        hashFilename = path;
      });
    } else {}
  }

  void _pickMD5FileToHash() async {
    FilePickerResult? result = await FilePicker.platform.pickFiles(type: FileType.any, dialogTitle: "Select file for hash generating");
    if (result != null) {
      String path = result.files.single.path;
      setState(() {
        fileToCountHashFilename = path;
      });
    } else {}
  }

  void _generateHashFromFile() async {
    FilePickerResult? result = await FilePicker.platform.pickFiles();

    if (result != null) {
      //File file = File(result.files.single.path);
      String path = result.files.single.path;
      var resultHash = await new MD5Hasher().generateMD5HashFromFile(path);
      setState(() {
        hashCodeGenerated = "$resultHash";
      });
      var saveFilePath = await FilePicker.platform.getDirectoryPath(dialogTitle: "select path to save generated hash");
      if (saveFilePath != null) {
        var filename = result.files.single.name;
        var index = filename.lastIndexOf(".");
        var name = filename.substring(0, index);
        saveFilePath += "/" + name + ".md5";
        var file = new File(saveFilePath);
        file.writeAsString(resultHash);
        _showMyDialog(saveFilePath);
      }
    } else {
      // User canceled the picker
    }
  }

  void _generateHashAndCompare() async {
    if (hashFilename == "" || fileToCountHashFilename == "") {
      _showMyDialog("can`t compare. \n Please, select both file which stores md5 hash and file you want to calculate hash");
    } else {
      var hashCounted = await new MD5Hasher().generateMD5HashFromFile(fileToCountHashFilename);
      var hash = new File(hashFilename).readAsStringSync().toLowerCase();

      if (hashCounted == hash) {
        _showMyDialog("Provided hash and hash of file are equal");
      } else {
        _showMyDialog("Provided hash and hash of file are not equal");
      }
    }
  }

  Future<void> _showMyDialog(String textData) async {
    //String content = textData + "";
    return showCupertinoDialog(
      context: context,
      barrierDismissible: false, // user must tap button!
      builder: (BuildContext context) {
        return CupertinoAlertDialog(
          title: const Text('Comparing'),
          content: SingleChildScrollView(
            child: ListBody(
              children: <Widget>[
                Text(textData),
              ],
            ),
          ),
          actions: <Widget>[
            TextButton(
              child: const Text('Ok'),
              onPressed: () {
                Navigator.of(context).pop();
              },
            ),
          ],
        );
      },
    );
  }
}
