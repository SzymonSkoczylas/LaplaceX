.data
	LAPLACE_MASK SDWORD -1, 0, -1, 0, 4, 0, -1, 0, -1

.code
ApplyFilterAsm proc EXPORT
	
	local iter_x: QWORD
	local iter_y: QWORD
	local x_max: QWORD
	local y_max: QWORD
	local widthX: QWORD
	local height: QWORD
	local output: QWORD
	local image: QWORD
	local sumR: SDWORD 
    local sumG: SDWORD 
    local sumB: SDWORD 

	mov sumR, 0							
	mov sumG, 0
	mov sumB, 0
	mov iter_x, 0
	mov iter_y, 0
	XOR r12, r12

; VXORPS ymm0, ymm0, ymm0
; Set all elements in YMM0 register to 0 by performing a bitwise exclusive OR (XOR)
; operation between YMM0 and itself.

; VXORPS ymm1, ymm1, ymm1
; Set all elements in YMM1 register to 0 by performing a bitwise exclusive OR (XOR)
; operation between YMM1 and itself.

; VXORPS ymm2, ymm2, ymm2
; Set all elements in YMM2 register to 0 by performing a bitwise exclusive OR (XOR)
; operation between YMM2 and itself.

; VXORPS ymm3, ymm3, ymm3
; Set all elements in YMM3 register to 0 by performing a bitwise exclusive OR (XOR)
; operation between YMM3 and itself.

; VMOVUPS ymm3, LAPLACE_MASK+4
; Load the value from the memory address LAPLACE_MASK+4 into the YMM3 register.

; VCVTDQ2PS ymm3, ymm3
; Convert the integer values in YMM3 to single-precision floating-point values.

	VXORPS ymm0, ymm0, ymm0
	VXORPS ymm1, ymm1, ymm1
	VXORPS ymm2, ymm2, ymm2
	VXORPS ymm3, ymm3, ymm3
	VMOVUPS ymm3, LAPLACE_MASK+4
	VCVTDQ2PS ymm3, ymm3


	mov image, rcx						
	mov rax, 3 						    
	mul rdx
	mov widthX, rax
	mov height, r8
	mov output, r9					
					
	sub rax, 6							
	mov x_max, rax
	sub r8, 2
	mov y_max, r8

LOOP_Y:									
	mov rcx, 0
	inc iter_y
	mov iter_x, 0

LOOP_X:									
	add iter_x, 3

	mov sumR, 0						
	mov sumG, 0
	mov sumB, 0

; -------------------- TOP LEFT ----------------------------			

	mov rax, iter_y		
	dec rax
	mul widthX
	add rax, iter_x
	add rax, image 
	sub rax, 1
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm0, r12d
	pslldq xmm0, 4

	sub rax, 1			
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm1, r12d
	pslldq xmm1, 4

	sub rax, 1			
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm2, r12d
	pslldq xmm2, 4

; -------------------- TOP MIDDLE ----------------------------
	add rax, 5
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm0, r12d
	pslldq xmm0, 4

	dec rax
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm1, r12d
	pslldq xmm1, 4

	dec rax
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm2, r12d
	pslldq xmm2, 4

; -------------------- TOP RIGHT ----------------------------

	add rax, 5
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm0, r12d
	pslldq xmm0, 4

	dec rax
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm1, r12d
	pslldq xmm1, 4

	dec rax
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm2, r12d
	pslldq xmm2, 4


; -------------------- MIDDLE LEFT ----------------------------

	mov rax, iter_y	
	mul widthX
	add rax, iter_x
	sub rax, 1				
	add rax, image

	mov r12b, byte ptr [rax]
	CVTSI2SS xmm0, r12d

	sub rax, 1			
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm1, r12d

	sub rax, 1			
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm2, r12d


	;YMMM shift
	VPERM2F128 ymm0, ymm0, ymm0, 1
	VPERM2F128 ymm1, ymm1, ymm1, 1
	VPERM2F128 ymm2, ymm2, ymm2, 1

; -------------------- MIDDLE MIDDLE ----------------------------
	add rax, 5
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm0, r12d
	pslldq xmm0, 4

	dec rax
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm1, r12d
	pslldq xmm1, 4
	
	dec rax
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm2, r12d
	pslldq xmm2, 4


; -------------------- MIDDLE RIGHT ----------------------------

	add rax, 5
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm0, r12d
	pslldq xmm0, 4

	dec rax
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm1, r12d
	pslldq xmm1, 4
	
	dec rax
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm2, r12d
	pslldq xmm2, 4

; -------------------- BOTTOM LEFT ----------------------------
	
	mov rax, iter_y		
	inc rax				
	mul widthX
	add rax, iter_x
	sub rax, 1
	add rax, image

	mov r12b, byte ptr [rax]
	CVTSI2SS xmm0, r12d
	pslldq xmm0, 4
	sub rax, 1			

	mov r12b, byte ptr [rax]
	CVTSI2SS xmm1, r12d
	pslldq xmm1, 4
	sub rax, 1			

	mov r12b, byte ptr [rax]
	CVTSI2SS xmm2, r12d
	pslldq xmm2, 4
; -------------------- BOTTOM MIDDLE ----------------------------
	add rax, 5

	mov r12b, byte ptr [rax]
	CVTSI2SS xmm0, r12d

	sub rax, 1			
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm1, r12d

	sub rax, 1			
	mov r12b, byte ptr [rax]
	CVTSI2SS xmm2, r12d

	; -------------------- CALCULATE VALUES ----------------------------

	VMULPS YMM0, YMM0, YMM3
	VMULPS YMM1, YMM1, YMM3
	VMULPS YMM2, YMM2, YMM3
	VCVTPS2DQ YMM0, YMM0
	VCVTPS2DQ YMM1, YMM1
	VCVTPS2DQ YMM2, YMM2

	xor rax, rax
    
	mov rcx, 4
	get_lower_lane:
	PEXTRD eax, xmm0, 0
	psrldq xmm0, 4
	add sumB, eax
	PEXTRD eax, xmm1, 0
	psrldq xmm1, 4
	add sumG, eax
	PEXTRD eax, xmm2, 0
	psrldq xmm2, 4
	add sumR, eax
	loop get_lower_lane 

	;YMMM shift
	VPERM2F128 ymm0, ymm0, ymm0, 1
	VPERM2F128 ymm1, ymm1, ymm1, 1
	VPERM2F128 ymm2, ymm2, ymm2, 1
	mov rcx, 4
	get_upper_lane:
	PEXTRD eax, xmm0, 0
	psrldq xmm0, 4
	add sumB, eax
	PEXTRD eax, xmm1, 0
	psrldq xmm1, 4
	add sumG, eax
	PEXTRD eax, xmm2, 0
	psrldq xmm2, 4
	add sumR, eax
	loop get_upper_lane 

	;rest of algorithm

	mov rax, iter_y	
	inc rax				
	mul widthX
	add rax, iter_x
	add rax, 5
	add rax, image
	mov r10b, [rax]
	xor rax, rax
	mov al, r10b
	imul [LAPLACE_MASK + 32]
	add sumB, eax

	mov rax, iter_y	
	inc rax				
	mul widthX
	add rax, iter_x
	add rax, 4
	add rax, image
	mov r10b, [rax]
	xor rax, rax
	mov al, r10b
	imul [LAPLACE_MASK + 32]
	add sumG, eax


	mov rax, iter_y	
	inc rax				
	mul widthX
	add rax, iter_x
	add rax, 3				
	add rax, image
	mov r10b, [rax]
	xor rax, rax
	mov al, r10b
	imul [LAPLACE_MASK + 32]
	add sumR, eax

	; normalize - starting with RED
	
	xor rax, rax
	mov eax, sumR
	cmp sumR, 255		
	JL LESS_R
	mov eax, 255		
	mov sumR, eax
	JMP GREEN
LESS_R:
	cmp eax, 0			
	JG GREEN
	mov eax, 0			
	mov sumR, eax


GREEN:
	xor rax, rax
	mov eax, sumG
	cmp sumG, 255		
	JL LESS_G
	mov eax, 255
	mov sumG, eax
	JMP BLUE
LESS_G:
	cmp eax, 0
	JG BLUE
	mov eax, 0
	mov sumG, eax

BLUE:
	xor rax, rax
	mov eax, sumB
	cmp sumB, 255		
	JL LESS_B
	mov eax, 255
	mov sumB, eax
	JMP SAVE
LESS_B:
	cmp eax, 0
	JG SAVE
	mov eax, 0
	mov sumB, eax

SAVE:					

	mov rcx, 3
	; saving RED value
	mov rax, iter_y		
	mul widthX		
	add rax, iter_x		
	add rax, 0				
	add rax, output		
	mov r10d, sumR
	mov [rax], r10b			

	; saving GREEN value
	mov rax, iter_y		
	mul widthX
	add rax, iter_x
	add rax, 1				
	add rax, output
	mov r10d, sumG
	mov [rax], r10b

	; saving BLUE value
	mov rax, iter_y		
	mul widthX
	add rax, iter_x
	add rax, 2				
	add rax, output
	mov r10d, sumB
	mov [rax], r10b

	mov rax, iter_x		
	cmp rax, x_max
	JB LOOP_X

	mov rax, iter_y		
	cmp rax, y_max
	JB LOOP_Y

	ret

ApplyFilterAsm endp

end