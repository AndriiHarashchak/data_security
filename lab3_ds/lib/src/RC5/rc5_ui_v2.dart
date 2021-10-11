import 'dart:convert';
import 'dart:typed_data';

import 'package:convert/convert.dart';
import 'package:flutter/material.dart';
import 'package:lab3_ds/src/RC5/md5_hasher.dart';
import 'package:lab3_ds/src/RC5/rc5_encryptor_decryptor.dart';

class RC5Page extends StatefulWidget {
  const RC5Page({Key? key}) : super(key: key);

  @override
  _RC5PageState createState() => _RC5PageState();
}

class _RC5PageState extends State<RC5Page> {
  final TextEditingController _inputDataController = TextEditingController();
  final TextEditingController _keyController = TextEditingController();
  String result = "";
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Container(
        child: Column(
          children: [
            Row(
              children: [
                Expanded(
                  child: TextField(
                    controller: _inputDataController,
                    decoration: InputDecoration(border: OutlineInputBorder(), labelText: "Input data"),
                  ),
                  flex: 3,
                ),
                Expanded(
                  child: Padding(
                    padding: EdgeInsets.all(5),
                    child: ElevatedButton(
                      onPressed: () => {encodeText()},
                      child: Text("Encode from text"),
                    ),
                  ),
                  flex: 1,
                ),
              ],
            ),
            Row(
              children: [
                Expanded(
                  child: Padding(
                    padding: EdgeInsets.all(5),
                    child: TextField(
                      controller: _keyController,
                      decoration: const InputDecoration(border: OutlineInputBorder(), labelText: "Key text"),
                    ),
                  ),
                  flex: 3,
                ),
                Expanded(
                  child: ElevatedButton(
                    child: Text("Select file"),
                    onPressed: () => {},
                  ),
                  flex: 2,
                ),
              ],
            ),
            Padding(
              padding: EdgeInsets.all(10),
              child: ElevatedButton(
                child: Text("Encode from file"),
                onPressed: () => {},
              ),
            ),
            Container(
              width: double.infinity,
              decoration: BoxDecoration(
                border: Border.all(width: 2.0, color: Colors.white),
                borderRadius: BorderRadius.all(Radius.circular(5)),
              ),
              child: Expanded(
                child: Text(
                  result,
                  style: TextStyle(),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  void encodeText() async {
    var input = _inputDataController.text;
    var key = _keyController.text;

    var inputList = utf8.encode(input);
    var inputKey = utf8.encode(key);
    MD5Hasher hasher = MD5Hasher();
    var hashedKey = hasher.generateMD5Hash(key);
    // List<int> correctKey = List.empty(growable: true);
    // for (int i = 0; i < 16; i += 2) {
    //   var code = hex.decode(hashedKey[i] + hashedKey[i + 1]);
    //   correctKey.addAll(code);
    //   //correctKey.add();
    // }
    var correctKey = hashedKey.sublist(0, 8); //key size
    RC5 rc5 = RC5(Uint8List.fromList(correctKey), 8); //rounds count
    var encryptedData = rc5.encrypt_decrypt(Uint8List.fromList(inputList), EncryptionType.Encrypt);
    var resultCode = rc5.encrypt_decrypt(encryptedData, EncryptionType.Decrypt);
    setState(() {
      result = inputList.toString() + " " + resultCode.toString();
    });
  }
}
