﻿* = $2000

;40 * 1,2,3,1,2,3,...
!fill 40,[1,2,3]

PI = 3.14159265359
!fill 256,math.sin(( i * PI * 2)*360/(2*PI)/256) * 127+127
;!fill 256,math.sin(( i * 3.14159265359 * 2)/256) * 127+127