;
;  PDS Pc1.21 :000: (c) P.D.Systems Ltd 1985-88
;
;
;       File  " IN-GAME ROUTINES! "
;    Version          09.64
;         By  "THIS IS STILL JAZ.R"
;
;     Created on Sat the 14th of May 1988
;        Last update 08:36 on 25/04/90
;

SEQDATA    EQU    $A000
SEQBYTES    EQU    15

ORG    SEQDATA-SEQBYTES

ENDCODE

  ORG ENDCODE
  ;ORG $1000


SCOL1 DB  0,6,14,3,13,7,10,8,9,0
SCOL2 DB  0,6,14,3,13,7,10,8,9,0
SDEL1 DB  7,7,7,7,0,8,8,6
SDEL2 DB  8,8,8,8,7,8,8,6
SDEL4 DB  8,0,13

;SDEL3  DB  0,4,8,6,7,8,7,9
SDEL3 DB  0,6,6,6,6,7,7,7
SCOL5 DB  0,0,11,12,15,7,1,1
SCOL6 DB  0,0,8,10,15,7,1,1
SCOL7 DB  14,4,6
SCOL8 DB  0,6,4

SDEL5 DB  7,8,11
SCOL9 DB  10,8,9
SCOL10  DB  0,0,4,14,3,13,1,1

SDEL3B  DB  2,8,6,5,6,6,7,6
SDEL2B  DB  19,7,7
SCOL9B  DB  0,9,8


SMUSIC  LDY #7
!L2 LDA SCOL1,Y
 STA $D020
  STA $D021
  LDX SDEL1,Y
!L1 DEX
  BPL !L1
  DEY
  BPL !L2
  LDX #6
!SIC  LDA ACPC,X
  STA $D027,X
  DEX
  BPL !SIC
  LDA #13
  STA $D026
  LDA #6
  STA $D025

  INC COLFLAG

  LDY #2
!L2B  LDX SDEL4,Y
!L1B  DEX
  BPL !L1B
  LDA SCOL7,Y
  STA $D020
  STA $D021
  DEY
  BPL !L2B

  LDA #$49
  LDY #BAND00&255
  LDX #BAND00/256
  RTS



BAND00  LDA #9
  STA $D025
  LDA #7
  STA $D026
  LDX #6
!SIC  LDA ACPC2,X
  STA $D027,X
  DEX
  BPL !SIC

  LDA #1
  STA RCNT
  LDA #$5A
  LDY #BAND0&255
  LDX #BAND0/256
  RTS


BAND0 LDX #1
!DJ DEX
  BPL !DJ

  LDY #7
!L2 LDA SCOL5,Y
  STA $D022
  LDA SCOL5,Y
  STA $D023
  LDX SDEL3B,Y
!L1 DEX
  BPL !L1
  DEY
  BPL !L2
  DEC RCNT
  BMI !E
  LDA #$6A
  LDY #BAND0&255
  LDX #BAND0/256
  RTS

!E  LDY #2
!L2B  LDX SDEL5,Y
!L1B  DEX
  BPL !L1B
  LDA SCOL8,Y
  STA $D020
  STA $D021
  DEY
  BPL !L2B

  LDA #$7A+8
  LDY #BAND0B&255
  LDX #BAND0B/256
  RTS


BAND0B  LDY #7
!L2 LDA SCOL6,Y ;name, hex bytes
  STA $D022
  LDA SCOL6,Y
  STA $D023
  LDX SDEL3C,Y
!L1 DEX
  BPL !L1
  DEY
  BPL !L2

  LDA #>SCOL3
  STA SPLIT1+1
  LDA #>SCOL4
  STA SPLIT2+1
  LDA #$92
  LDY #BAND1&255
  LDX #BAND1/256
  RTS

COLFLAG DB  0
RCNT  DB  0
¡
SPLIT LDY #5
SPLIT1  LDA SCOL3,Y
  STA $D022
SPLIT2  LDA SCOL4,Y
  STA $D023
  LDX SDEL3,Y
!L1 DEX
  BPL !L1
  DEY
  BPL SPLIT1
  INC SPLIT1+1
  INC SPLIT2+1
  RTS

BAND1 JSR SPLIT
  LDA #$9A
  LDY #BAND1B&255
  LDX #BAND1B/256
  RTS
BAND1B  JSR SPLIT
  LDA #$A2
  LDY #BAND1C&255
  LDX #BAND1C/256
  RTS
BAND1C  JSR SPLIT
  LDA #$AA
  LDY #BAND1D&255
  LDX #BAND1D/256
  RTS
BAND1D  JSR SPLIT
  LDA #$B2
  LDY #BAND1E&255
  LDX #BAND1E/256
  RTS
BAND1E  JSR SPLIT
  LDA #$BA
  LDY #BAND1F&255
  LDX #BAND1F/256
  RTS
BAND1F  JSR SPLIT
  LDA #$C2
  LDY #BAND1G&255
  LDX #BAND1G/256
  RTS
BAND1G  JSR SPLIT
  LDA #$D2
  LDY #BAND2&255
  LDX #BAND2/256
  RTS

BAND2 LDY #7
!L2 LDA SCOL10,Y
  STA $D022
  LDA SCOL10,Y
  STA $D023
˜™  LDX SDEL3C,Y
!L1 DEX
  BPL !L1
  DEY
  BPL !L2
  LDA #$E6
  LDY #BAND3&255
  LDX #BAND3/256
  RTS


BAND3 LDY #2
!L2B  LDA SCOL9,Y
  STA $D020
  STA $D021
  LDX SDEL2B,Y
!L1B  DEX
  BPL !L1B
  DEY
  BPL !L2B

  LDX #1
!D  DEX
  BPL !D

  LDY #7
!L2 LDA SCOL5,Y
  STA $D022
  LDA SCOL5,Y
  STA $D023
  LDX SDEL3B,Y
!L1 DEX
  BPL !L1
  DEY
  BPL !L2

  LDY #7
!L2C  LDA SCOL5,Y
  STA $D022
  LDA SCOL5,Y
  STA $D023
  LDX SDEL3B,Y
!L1C  DEX
  BPL !L1C
  DEY
  BPL !L2C

!E  LDX #14
!DJ DEX
  BPL !DJ

  LDY #2
!L2D  LDA SCOL9B,Y
  STA $D020
  STA $D021
  LDX SDEL2B,Y
!L1D  DEX
  BPL !L1D
  DEY
  BPL !L2D

; LDA #$FF
; LDY #SMUSIC2&255
; LDX #SMUSIC2/256
; RTS

  LDX #13
!D2 DEX
  BPL !D2

SMUSIC2 LDY #7
!L2 LDA SCOL2,Y
  STA $D020
  STA $D021
  LDX SDEL2,Y
!L1 DEX
  BPL !L1
  DEY
  BPL !L2

  JSR STARFIELD

  DEC FC
  BPL !REND
  LDA #2
  STA FC
  LDX #0
  SEC
  JSR ROLLRASTS
  LDX #1
  CLC
  JSR ROLLRASTS

  LDA PULSEF
  BNE !REND
  LDX CURRENTM
  CPX #8
  BEQ !REND
  LDA COLLO,X
  STA !S+1
  STA !CL+1
  LDA COLHI,X
  STA !S+2
  STA !CL+2
  LDX COLOFF
  DEX
  BPL !OK
  LDX #9
!OK STX COLOFF
  LDY #39
!CL LDA $FFFF,Y
  AND #15
  CMP #1
  BEQ !E
  LDA COLTAB,X
!S  STA $FFFF,Y
!E  DEY
  BPL !CL

!REND DEC FC2
  BPL !REND2
  LDA #2
  STA FC2
  LDX #2
  SEC
  JSR ROLLRASTS
  LDX #3
  CLC
  JSR ROLLRASTS

!REND2  LDA #$2F
  LDY #SMUSIC&255
  LDX #SMUSIC/256
  RTS


ROLLRASTS LDA RLO2,X  ;x  : offset
  STA $54 ;sec: down
  LDA RHI2,X  ;clc: up
  STA $55
  LDY RSZ,X ;length
  STY $56
  BCS !DOWN

!UP LDA ($54),Y
  PHA
  LDY $56
  DEY
!UL LDA ($54),Y
  INY
  STA ($54),Y
  DEY
  DEY
  BPL !UL
  INY
  PLA
  STA ($54),Y
  RTS

!DOWN LDY #0
  LDA ($54),Y
  PHA
  INY
!RU LDA ($54),Y
  DEY
  STA ($54),Y
  INY
  INY
  CPY $56
  BCC !RU
  DEC $56
  LDY $56
  PLA
  STA ($54),Y
  RTS




DECOMP  STA TT+3
; BMI !2
; LDA #>WINDOW
; STA TT+6
; LDA #<WINDOW
; STA TT+7
; BNE !3
!2  LDA #>WINDOW2
  STA TT+6
  LDA #<WINDOW2
  STA TT+7
!3  STX !U1+1
  STY !U3+1
  LDA #$44
  STA !B+2
  LDA #0
  STA !B+1

!A  LDX #0
  LDY #0
  LDA (TT+6),Y
  CMP TT+3
  BNE !S
  INY
  LDA (TT+6),Y
  TAX
  DEX
  LDA TT+3
!S  STX TT+1
  INC TT+1
!B  STA $4400,X
  DEX
  BPL !B
  CLC
  LDA !B+1
  ADC TT+1
  STA !B+1
  BCC !OK
  INC !B+2
!OK INY
  STY !D+1
  CLC
  LDA TT+6
!D  ADC #$FF
  STA TT+6
  BCC !E
  INC TT+7
!E  LDA TT+7
!U3 CMP #$FF
  BCC !A
  LDA TT+6
!U1 CMP #$FF
  BCC !A
  RTS


SDEL3C  DB  7,7,7,7,7,8,8,9


STARTMUSIC


MUSIC1  HEX 9F01B4030001010F2501020F0A3A04060E4115D20311070505061100B2080010
  HEX 1900B208BE0800101900B20800081900BE0800081900B208BE0800081900B208
  HEX B50810101100B2080008190023690500B208BE0814F423690500140023690500
  HEX B20823690500BE0814FE236905001400B208BE0814FE236905001400B208B508
  HEX B20814E823690500140023690500B208BE082369050014FE236905001400B208
  HEX 03530701A10807050311BE0803530701A10807050311B208BE08035307019F08
  HEX 07050311B208B5081019020F0A3A04060E4115D20311070505061100B908B508
  HEX 00081900B408B50800081900B40800081900B908B50800081900B408B5080008
  HEX 1900B0080008190010061100B908B508BE08B408B508BE08B408C108B908B508
  HEX BE08B408B508C108B008C008100D1100B208BE08C108C308BE08B208BC08B208
  HEX BE08B208B208BE08B208BE08B208C50810040EFF040F0700034102AA0A7A0F01
  HEX 0D500C00097822C0C1C0094022BEBC4019011100098022B9BCC0094022BEBC40
  HEX 1901098022C0C1C0094022BEBC401901098022B7B9C0094022B2B54019011020
  HEX 00011E000182F802090A0B040F0EFF15D2031107020D2C0C010F010906A10811
  HEX 00A408A608B50800081900A608B208B70800081900A408A608B50800081900A6
  HEX 08B2080A78090EA91009060A0910081100A408A608B508020F0700C101C801CD
  HEX 01C801C101C10307020209A608B208B708020F0700BC01C001C501C001BC01C0
  HEX 0307010209A408A608B508020F0700BE01C101C801C101BE01C10307020209A6
  HEX 08B2080A78090EA91009060A09102E020A0A9A034107000F001100B908B708B9
  HEX 08BC08B908BE08B908BE08BC08BE08B908BC08B908B708B908BC08BE08BC08BE
  HEX 08C108BE08C108BE08C108C008C108BC08C008BC08B908BC08B70810041100B2
  HEX 08B008B208B708B208B908B208B908B708B908B208B708B208B008B208B70810
  HEX 02040F020D0E500A9A0341229A9A00190198809D689C189A001901A1809F80A1
  HEX 80A4D00D900C0109000F010408236005009A0823600500236005009A08236005
  HEX 00040F0D3C0C0009E0A6D0190104080900229A0D900C01236005009A08236005
  HEX 00236005009A08236005000F00040F11009A08A608A608A408A608A108A408A1
  HEX 0810079A08A6080F01040823600500A4082360050023600500A4082360050002
  HEX 0C0A8A1100229A23570500A61823600500A908A608A41823570500A608236005
  HEX 00229FA908A610229A23570500A61823600500A90823570500A618229D235705
  HEX 00A90823600500229FA9082360050023600500102000011E00018200190202DF
  HEX 0A4A04080E4115D20341130011009A0019029D0019019A001902980019011002
  HEX 9A0019019D001901A10019019F001901A1001901A40019010A0AA60019040D90
  HEX 0C01020A0A8A070003410F011100229A23570500001819002360050000281900
  HEX 23570500000819002360050000181900229D2357050000181900236005000008
  HEX 19002357050000181900229C2357050000081900236005000018190010021100
  HEX 229A23570500A61823600500A908A608A41823570500A60823600500A908A610
  HEX 229D23570500A61823600500A90823570500A618229C23570500A60823600500
  HEX A908A6082360050010060F00A6001901140C0010190002DA11009A0019019880
  HEX 9D689C189A001901A1809F80A180A4F0020911009A08A608A608A408A608A108
  HEX A408A10810101400040F0381070105080EE11100238405001004070311002384
  HEX 05001008070111002384050010080705040F0E0011002384050010080E500700
  HEX 0341140C0AAA11009D401BFF9D2898181BFF9A801BFF98401BFF982895181BFF
  HEX 9A801BFF10021BFF95401BFF98401BFF9A00190100011E0381BE010341A90724
  HEX 0381C5010341AD072407000311020ABE01C101C501BE01C101C501CA02020F03
  HEX 11070524BE08C108C308BE08B208BC08B208BE08B208B208BE08B208BE08B208
  HEX C508C10824



MUSIC2  HEX 0501D201000125031402011F02A90A0D0341040815FA1800
  HEX 1D0517C511009A209A209D209D20982098209A209510980C
  HEX 9F0410020A8D9A7C98049A3898089A36010F17000E640209
  HEX 0A090DC20C019D049D029F0411009A029A049A029D049D02
  HEX 9F049F049D049D029F0410081300110023B902001006020A
  HEX 0A0C9A7C98049A3898089A3C98049A3898089A209A189D08
  HEX 9A209A14980C9A209A169D049D029F0411009A1C98049A0E
  HEX 9F049F049D049D029F0410039A1C02090A0998049A0E9F02
  HEX 0F03AD020F009F049D020F03AD020F009D020F03AD020F00
  HEX 9F02110023B90200100411009A029A049A029D049D029F04
  HEX 9F049D049D029F041010110023B902001003020D9A800001
  HEX 1E0001140202660A180357040615D20EFF0BC81602B2FA03
  HEX 410B0002070A0714F69A029A02A602130023490200140212
  HEX 04237203000D960C000F0014020E64237203001100237D03
  HEX 00100202080A080EFF1F0C0801110023A002001010080023
  HEX 490200237203001F001100237D030010020F00A68002091F
  HEX 0C1100A608100EAD08B004B40413001100B208B208B908B7
  HEX 08B208B208B708B908B208B208B908B708B208B904B704B5
  HEX 08B708100208010BC8160221141202020D0A6A0D320C000F
  HEX 01229ABE800F000A0ABE8000011E00011402008019001F0C
  HEX 034115D202340A440E0A04029A8002A20A479A0019010311
  HEX 040F0EFF02A20A0700021900140E11002349020010040341
  HEX 1402040A02080A08130023490200120414F6080113001100
  HEX 23A002001008140E1202140208001300234902001206020C
  HEX 0801B200190100011E1100B202B902BE02100AB202B90211
  HEX 00B502BC02C102100AB502BC021100B002B702BC02100AB0
  HEX 02B7021100B202B902BE021005B202AD02B202B902AD02B2
  HEX 02B902AD02B202B002B702BC02B002B702BC02B002B70224
  HEX B204AD02A604B202B902B002B004B202B904BC02BE02B902
  HEX 240F03A6020F009A049A020F03AD020F009D029D029F020F
  HEX 03A6020F009F049D020F03AD020F009D029F029F020F03A6
  HEX 020F009A049A020F03AD020F009D029D029F020F03A6020F
  HEX 009F049D020F03AD020F009D029F020F03AD020F000F03A6
  HEX 020F009A049A020F03AD020F009D029D029F020F03A6020F
  HEX 009F049D020F03AD020F009D029F029F020F03A6020F009A
  HEX 049A020F03AD020F009D029D029F020F03A6020F009F049D
  HEX 020F03AD020F009D020F03AD02AD020F00241F001402026C
  HEX 0A2C040A24B20C0F0122B0AB080F00B0040F0122B5B0080F
  HEX 00B214B004B204B004AD100F0122B0AB080F00AD04B0040F
  HEX 0122B2AD200F00B20C0F0122B0AB080F00B0040F0122B5B0
  HEX 080F00B214B004B204B004B5100F0122B4AF080F00B504B0
  HEX 040F0122B2AD200F00240122B2AD200F0024B5B0080F00B2



MUSIC3  HEX 35004E01000117C10764011F2501020C0A0A040F034115D20E1418501D0D9AC0
  HEX 13009A80190123F70100120311001BFF00541900100F00011E000102090A8A03
  HEX 4115D20E460D580C020F01229A11002355020010101100234802001010130011
  HEX 000F01229A235502009A062369020023480200236202009A0523550200236202
  HEX 009A05229D235502009D06236202009D05236202009D0523480200236202009D
  HEX 0523690200229F23550200236202009F05236202009F05235502002348020023
  HEX 6202009F0523550200229D236202009D0523550200236202009D052369020022
  HEX 9F23480200236202009F052348020023480200100A1100229A235502009A0623
  HEX 5502009A06229D234802009D06229F23550200A1061006235502009F06235502
  HEX 009F06234802009F062355020023550200235502002348020023550200235502
  HEX 00234802009F06234802009F06122000011E000102090A08040F15D20E140341
  HEX 9A8019011F0C13009A0C110014FB238302001400B00C1400238302001400B00C
  HEX 1002110023830200B00C1405238302001400B00C10021100B20623720200B206
  HEX 23720200B2061407237202001400B206B206B00CB70623720200B7068212B706
  HEX 1405237202001400B706B706B00C1005A6741901A6C01202020C9AC00E1414F4
  HEX 110023F70100140003119AC008011002005019C300011EB960C30CC10C000C19
  HEX 00B50CB718B518B2C0B960C10CC30CB218B50CB718B50CB2C0AD60C30CC10C00
  HEX 0C1900AB0CAD18B018B2C0B260C10CC30CB218B50CB718B50CB00CB218B29CA6
  HEX 60A660A660AB602404080381CA010341AD05040F2404080381C5010341A60504
  HEX 0F240381D6010341240381DD0103419A0B240208034108018206B90C03410800
  HEX 020924B20600121900B20600121900B20600121900B206B20624



MUSIC4  HEX FF00A00100012501010F0764011F17C31D2818E6020D0ADD03410E3C15D20408
  HEX 0DBC0C0211009A089A08000819009A089D089D08000819009D089F089F080008
  HEX 19009F089F089D08000819009F081004110023D502009A08230903009A0823E4
  HEX 02009D08230903009D0823F302009F082309030023F302009F0823E402002309
  HEX 03009F0810041300110023D50200230203009A0723090300230203009A0723E4
  HEX 0200230203009D0723090300230203009D0723F30200230203009F0723090300
  HEX 23F30200230203009F0723E4020023090300230203009F071007110023020300
  HEX 00071900100C0DF40C0123E4020023E402002309030023E402000DBC0C021208
  HEX 00011E0001020C0A0A0381BE00190102090A8903410EFF15D204000DC80C190F
  HEX 0109070705050700001903110022B2BE0800081900BE08BE0800081900BE08BE
  HEX 08BE0800081900BE08BE08C10800081900BE08C1080008190010081300110023
  HEX 4303001008070305080F00040C1100B208B510B708BE08B708B508BE08B208B5
  HEX 10B708C508B508B708C30810100F010507070504001100234303001020000019
  HEX 0000011E000102080A7903410E9615D204001100A610A908A608A910AB08A908
  HEX AB1000281900AB081004110023220300100C03510A9A13001100B201BE01B901
  HEX C50110201100B501C101BC01C80110101100B701C301BE01CA01101012081100
  HEX 0311232203000351232203000321232203000351232203001002140C11002322
  HEX 0300100814001100236403000008190023640300000819002364030000081900
  HEX 2364030014032364030014002364030000081900236403000008190023640300
  HEX 000819002364030014FE2364030014001004110023640300B20823640300B208
  HEX 23640300B20823640300140323640300140023640300B20823640300B2082364
  HEX 0300B5082364030014FE23640300140010080341110023220300100814F40408
  HEX 110023640300B90823640300B2081BFF100F00011E0381C50103410F01229AAD
  HEX 070F00240381C50103410F01229DAD070F00240381C50103410F01229FAD070F
  HEX 00240381C5010341240381C50103411F0C08010F030709B20707000F0008001F
  HEX 0024A610A908A608A910AB08A908AB10000819000311C104C3140381C5080341
  HEX AB082422B2BE10BE0822B5BE10BE08BE0822B7BE10BE0822B5BE08C11022B7BE
  HEX 08C110240208070503410506B9080351020C070024



STOPM HEX 11001E000001010F2503232F0000A65019C300011E0001232F00009A5019C300
  HEX 011E0001232F0000A15019C300011E0341202002080A0A04080E4115D224




ENDMUSIC










