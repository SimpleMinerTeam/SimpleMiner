// Copyright 2017 Yurio Miyazawa (a.k.a zawawa) <me@yurio.net>
//
// This file is part of Gateless Gate Sharp.
//
// Gateless Gate Sharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Gateless Gate Sharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Gateless Gate Sharp.  If not, see <http://www.gnu.org/licenses/>.



#define SWAP64(x)	(((x) >> 56) | (((x) >> 40) & 0x000000000000FF00UL) | (((x) >> 24) & 0x0000000000FF0000UL) \
          | (((x) >>  8) & 0x00000000FF000000UL) | (((x) <<  8) & 0x000000FF00000000UL) \
          | (((x) << 24) & 0x0000FF0000000000UL) | (((x) << 40) & 0x00FF000000000000UL) | (((x) << 56) & 0xFF00000000000000UL))

#define SWAP32(x)	((((x) >> 24) & 0xFFU) | (((x) >> 8) & 0xFF00U) | (((x) << 8) & 0xFF0000U) | (((x) << 24) & 0xFF000000U))



#if (defined(__Tahiti__) || defined(__Pitcairn__) || defined(__Capeverde__) || defined(__Oland__) || defined(__Hainan__))
#define LEGACY
#endif

#if defined(__GCNMINC__)
uint2 amd_bitalign(uint2 src0, uint2 src1, uint2 src2)
{
    uint2 dst;
    __asm("v_alignbit_b32 %0, %2, %3, %4\n"
          "v_alignbit_b32 %1, %5, %6, %7"
          : "=v" (dst.x), "=v" (dst.y)
          : "v" (src0.x), "v" (src1.x), "v" (src2.x),
		    "v" (src0.y), "v" (src1.y), "v" (src2.y));
    return dst;
}
#define mem_fence barrier
#elif defined(cl_amd_media_ops) && !defined(__GCNMINC__)
#pragma OPENCL EXTENSION cl_amd_media_ops : enable
#elif defined(cl_nv_pragma_unroll)
uint amd_bitalign(uint src0, uint src1, uint src2)
{
    uint dest;
    asm("shf.r.wrap.b32 %0, %2, %1, %3;" : "=r"(dest) : "r"(src0), "r"(src1), "r"(src2));
    return dest;
}
#else
#define amd_bitalign(src0, src1, src2) ((uint) (((((ulong)(src0)) << 32) | (ulong)(src1)) >> ((src2) & 31)))
#endif

//#define LEGACY

#if WORKSIZE % 4 != 0
#error "WORKSIZE has to be a multiple of 4"
#endif

#define MAX_OUTPUTS 0xFFu
#define ACCESSES  64
#define FNV_PRIME 0x01000193U


static __constant uint2 const Keccak_f1600_RC[24] = {
    (uint2)(0x00000001, 0x00000000),
    (uint2)(0x00008082, 0x00000000),
    (uint2)(0x0000808a, 0x80000000),
    (uint2)(0x80008000, 0x80000000),
    (uint2)(0x0000808b, 0x00000000),
    (uint2)(0x80000001, 0x00000000),
    (uint2)(0x80008081, 0x80000000),
    (uint2)(0x00008009, 0x80000000),
    (uint2)(0x0000008a, 0x00000000),
    (uint2)(0x00000088, 0x00000000),
    (uint2)(0x80008009, 0x00000000),
    (uint2)(0x8000000a, 0x00000000),
    (uint2)(0x8000808b, 0x00000000),
    (uint2)(0x0000008b, 0x80000000),
    (uint2)(0x00008089, 0x80000000),
    (uint2)(0x00008003, 0x80000000),
    (uint2)(0x00008002, 0x80000000),
    (uint2)(0x00000080, 0x80000000),
    (uint2)(0x0000800a, 0x00000000),
    (uint2)(0x8000000a, 0x80000000),
    (uint2)(0x80008081, 0x80000000),
    (uint2)(0x00008080, 0x80000000),
    (uint2)(0x80000001, 0x00000000),
    (uint2)(0x80008008, 0x80000000),
};

#ifdef cl_amd_media_ops

#ifdef LEGACY
#define barrier(x) mem_fence(x)
#elif WORKSIZE <= 64
#error "WORKSIZE <= 64 isn't supported by newer AMD drivers and WORKSIZE > 64 is required"
#endif

#define ROTL64_1(x, y) amd_bitalign((x), (x).s10, 32 - (y))
#define ROTL64_2(x, y) amd_bitalign((x).s10, (x), 32 - (y))

#else

#define ROTL64_1(x, y) (uint2) { (uint)rotate(((ulong)((x).s1) << 32) | (x).s0, (ulong)(y)), (uint)rotate(((ulong)((x).s0) << 32) | (x).s1, (ulong)(y)) }
#define ROTL64_2(x, y) ROTL64_1(x, (y) + 32)

#endif


#define KECCAKF_1600_RND(a, i, outsz) do { \
    const uint2 m0 = a[0] ^ a[5] ^ a[10] ^ a[15] ^ a[20] ^ ROTL64_1(a[2] ^ a[7] ^ a[12] ^ a[17] ^ a[22], 1);\
    const uint2 m1 = a[1] ^ a[6] ^ a[11] ^ a[16] ^ a[21] ^ ROTL64_1(a[3] ^ a[8] ^ a[13] ^ a[18] ^ a[23], 1);\
    const uint2 m2 = a[2] ^ a[7] ^ a[12] ^ a[17] ^ a[22] ^ ROTL64_1(a[4] ^ a[9] ^ a[14] ^ a[19] ^ a[24], 1);\
    const uint2 m3 = a[3] ^ a[8] ^ a[13] ^ a[18] ^ a[23] ^ ROTL64_1(a[0] ^ a[5] ^ a[10] ^ a[15] ^ a[20], 1);\
    const uint2 m4 = a[4] ^ a[9] ^ a[14] ^ a[19] ^ a[24] ^ ROTL64_1(a[1] ^ a[6] ^ a[11] ^ a[16] ^ a[21], 1);\
    \
    const uint2 tmp = a[1]^m0;\
    \
    a[0] ^= m4;\
    a[5] ^= m4; \
    a[10] ^= m4; \
    a[15] ^= m4; \
    a[20] ^= m4; \
    \
    a[6] ^= m0; \
    a[11] ^= m0; \
    a[16] ^= m0; \
    a[21] ^= m0; \
    \
    a[2] ^= m1; \
    a[7] ^= m1; \
    a[12] ^= m1; \
    a[17] ^= m1; \
    a[22] ^= m1; \
    \
    a[3] ^= m2; \
    a[8] ^= m2; \
    a[13] ^= m2; \
    a[18] ^= m2; \
    a[23] ^= m2; \
    \
    a[4] ^= m3; \
    a[9] ^= m3; \
    a[14] ^= m3; \
    a[19] ^= m3; \
    a[24] ^= m3; \
    \
    a[1] = ROTL64_2(a[6], 12);\
    a[6] = ROTL64_1(a[9], 20);\
    a[9] = ROTL64_2(a[22], 29);\
    a[22] = ROTL64_2(a[14], 7);\
    a[14] = ROTL64_1(a[20], 18);\
    a[20] = ROTL64_2(a[2], 30);\
    a[2] = ROTL64_2(a[12], 11);\
    a[12] = ROTL64_1(a[13], 25);\
    a[13] = ROTL64_1(a[19],  8);\
    a[19] = ROTL64_2(a[23], 24);\
    a[23] = ROTL64_2(a[15], 9);\
    a[15] = ROTL64_1(a[4], 27);\
    a[4] = ROTL64_1(a[24], 14);\
    a[24] = ROTL64_1(a[21],  2);\
    a[21] = ROTL64_2(a[8], 23);\
    a[8] = ROTL64_2(a[16], 13);\
    a[16] = ROTL64_2(a[5], 4);\
    a[5] = ROTL64_1(a[3], 28);\
    a[3] = ROTL64_1(a[18], 21);\
    a[18] = ROTL64_1(a[17], 15);\
    a[17] = ROTL64_1(a[11], 10);\
    a[11] = ROTL64_1(a[7],  6);\
    a[7] = ROTL64_1(a[10],  3);\
    a[10] = ROTL64_1(tmp,  1);\
    \
    uint2 m5 = a[0]; uint2 m6 = a[1]; a[0] = bitselect(a[0]^a[2],a[0],a[1]); \
    a[0] ^= as_uint2(Keccak_f1600_RC[i]); \
    if (outsz > 1) { \
        a[1] = bitselect(a[1]^a[3],a[1],a[2]); a[2] = bitselect(a[2]^a[4],a[2],a[3]); a[3] = bitselect(a[3]^m5,a[3],a[4]); a[4] = bitselect(a[4]^m6,a[4],m5);\
        if (outsz > 4) { \
            m5 = a[5]; m6 = a[6]; a[5] = bitselect(a[5]^a[7],a[5],a[6]); a[6] = bitselect(a[6]^a[8],a[6],a[7]); a[7] = bitselect(a[7]^a[9],a[7],a[8]); a[8] = bitselect(a[8]^m5,a[8],a[9]); a[9] = bitselect(a[9]^m6,a[9],m5);\
            if (outsz > 8) { \
                m5 = a[10]; m6 = a[11]; a[10] = bitselect(a[10]^a[12],a[10],a[11]); a[11] = bitselect(a[11]^a[13],a[11],a[12]); a[12] = bitselect(a[12]^a[14],a[12],a[13]); a[13] = bitselect(a[13]^m5,a[13],a[14]); a[14] = bitselect(a[14]^m6,a[14],m5);\
                m5 = a[15]; m6 = a[16]; a[15] = bitselect(a[15]^a[17],a[15],a[16]); a[16] = bitselect(a[16]^a[18],a[16],a[17]); a[17] = bitselect(a[17]^a[19],a[17],a[18]); a[18] = bitselect(a[18]^m5,a[18],a[19]); a[19] = bitselect(a[19]^m6,a[19],m5);\
                m5 = a[20]; m6 = a[21]; a[20] = bitselect(a[20]^a[22],a[20],a[21]); a[21] = bitselect(a[21]^a[23],a[21],a[22]); a[22] = bitselect(a[22]^a[24],a[22],a[23]); a[23] = bitselect(a[23]^m5,a[23],a[24]); a[24] = bitselect(a[24]^m6,a[24],m5);\
            } \
        } \
    } \
    \
} while(0)


#define KECCAK_PROCESS(st, in_size, out_size, ethash_isolate)    do { \
    for (int r = 0;r < (23);) { \
        if (ethash_isolate) \
            KECCAKF_1600_RND(st, r++, 25); \
    } \
    KECCAKF_1600_RND(st, 23, out_size); \
} while(0)


#define fnv(x, y)        ((x) * FNV_PRIME ^ (y))
#define fnv_reduce(v)    fnv(fnv(fnv(v.x, v.y), v.z), v.w)

typedef union
{
    uint uints[128 / sizeof(uint)];
    ulong ulongs[128 / sizeof(ulong)];
    uint2 uint2s[128 / sizeof(uint2)];
    uint4 uint4s[128 / sizeof(uint4)];
    uint8 uint8s[128 / sizeof(uint8)];
    uint16 uint16s[128 / sizeof(uint16)];
    ulong8 ulong8s[128 / sizeof(ulong8)];
} hash128_t;


typedef union {
    ulong8 ulong8s[1];
    ulong4 ulong4s[2];
    uint2 uint2s[8];
    uint4 uint4s[4];
    uint8 uint8s[2];
    uint16 uint16s[1];
    ulong ulongs[8];
    uint  uints[16];
} compute_hash_share;


#define MIX(x) \
do { \
    buffer[get_local_id(0)] = fnv(init0 ^ (a + x), mix.s##x) % ethash_dag_size; \
    /*mem_fence(CLK_LOCAL_MEM_FENCE);*/ \
    mix = fnv(mix, g_dag[buffer[lane_idx]].uint8s[thread_id]); \
    mem_fence(CLK_LOCAL_MEM_FENCE); \
} while(0)



#if !defined(cl_khr_byte_addressable_store)
#error "Device does not support unaligned stores"
#endif


#define ROL32(x, n)  rotate(x, (uint) n)

#define SHR(x, n)    ((x) >> n)

#define S0(x) (ROL32(x, 25) ^ ROL32(x, 14) ^  SHR(x, 3))
#define S1(x) (ROL32(x, 15) ^ ROL32(x, 13) ^  SHR(x, 10))

#define S2(x) (ROL32(x, 30) ^ ROL32(x, 19) ^ ROL32(x, 10))
#define S3(x) (ROL32(x, 26) ^ ROL32(x, 21) ^ ROL32(x, 7))

#define P(a,b,c,d,e,f,g,h,x,K)                  \
{                                               \
    temp1 = h + S3(e) + F1(e,f,g) + (K + x);      \
    d += temp1; h = temp1 + S2(a) + F0(a,b,c);  \
}

#define F0(y, x, z) bitselect(z, y, z ^ x)
#define F1(x, y, z) bitselect(z, y, x)

#define R0 (W0 = S1(W14) + W9 + S0(W1) + W0)
#define R1 (W1 = S1(W15) + W10 + S0(W2) + W1)
#define R2 (W2 = S1(W0) + W11 + S0(W3) + W2)
#define R3 (W3 = S1(W1) + W12 + S0(W4) + W3)
#define R4 (W4 = S1(W2) + W13 + S0(W5) + W4)
#define R5 (W5 = S1(W3) + W14 + S0(W6) + W5)
#define R6 (W6 = S1(W4) + W15 + S0(W7) + W6)
#define R7 (W7 = S1(W5) + W0 + S0(W8) + W7)
#define R8 (W8 = S1(W6) + W1 + S0(W9) + W8)
#define R9 (W9 = S1(W7) + W2 + S0(W10) + W9)
#define R10 (W10 = S1(W8) + W3 + S0(W11) + W10)
#define R11 (W11 = S1(W9) + W4 + S0(W12) + W11)
#define R12 (W12 = S1(W10) + W5 + S0(W13) + W12)
#define R13 (W13 = S1(W11) + W6 + S0(W14) + W13)
#define R14 (W14 = S1(W12) + W7 + S0(W15) + W14)
#define R15 (W15 = S1(W13) + W8 + S0(W0) + W15)

#define RD14 (S1(W12) + W7 + S0(W15) + W14)
#define RD15 (S1(W13) + W8 + S0(W0) + W15)

/// generic sha transform
inline uint8 sha256_Transform(uint16 data, uint8 state)
{
	uint temp1;
	uint8 res = state;
	uint W0 = data.s0;
	uint W1 = data.s1;
	uint W2 = data.s2;
	uint W3 = data.s3;
	uint W4 = data.s4;
	uint W5 = data.s5;
	uint W6 = data.s6;
	uint W7 = data.s7;
	uint W8 = data.s8;
	uint W9 = data.s9;
	uint W10 = data.sA;
	uint W11 = data.sB;
	uint W12 = data.sC;
	uint W13 = data.sD;
	uint W14 = data.sE;
	uint W15 = data.sF;

#define v0  res.s0
#define v1  res.s1
#define v2  res.s2
#define v3  res.s3
#define v4  res.s4
#define v5  res.s5
#define v6  res.s6
#define v7  res.s7

	P(v0, v1, v2, v3, v4, v5, v6, v7, W0, 0x428A2F98);
	P(v7, v0, v1, v2, v3, v4, v5, v6, W1, 0x71374491);
	P(v6, v7, v0, v1, v2, v3, v4, v5, W2, 0xB5C0FBCF);
	P(v5, v6, v7, v0, v1, v2, v3, v4, W3, 0xE9B5DBA5);
	P(v4, v5, v6, v7, v0, v1, v2, v3, W4, 0x3956C25B);
	P(v3, v4, v5, v6, v7, v0, v1, v2, W5, 0x59F111F1);
	P(v2, v3, v4, v5, v6, v7, v0, v1, W6, 0x923F82A4);
	P(v1, v2, v3, v4, v5, v6, v7, v0, W7, 0xAB1C5ED5);
	P(v0, v1, v2, v3, v4, v5, v6, v7, W8, 0xD807AA98);
	P(v7, v0, v1, v2, v3, v4, v5, v6, W9, 0x12835B01);
	P(v6, v7, v0, v1, v2, v3, v4, v5, W10, 0x243185BE);
	P(v5, v6, v7, v0, v1, v2, v3, v4, W11, 0x550C7DC3);
	P(v4, v5, v6, v7, v0, v1, v2, v3, W12, 0x72BE5D74);
	P(v3, v4, v5, v6, v7, v0, v1, v2, W13, 0x80DEB1FE);
	P(v2, v3, v4, v5, v6, v7, v0, v1, W14, 0x9BDC06A7);
	P(v1, v2, v3, v4, v5, v6, v7, v0, W15, 0xC19BF174);

	P(v0, v1, v2, v3, v4, v5, v6, v7, R0, 0xE49B69C1);
	P(v7, v0, v1, v2, v3, v4, v5, v6, R1, 0xEFBE4786);
	P(v6, v7, v0, v1, v2, v3, v4, v5, R2, 0x0FC19DC6);
	P(v5, v6, v7, v0, v1, v2, v3, v4, R3, 0x240CA1CC);
	P(v4, v5, v6, v7, v0, v1, v2, v3, R4, 0x2DE92C6F);
	P(v3, v4, v5, v6, v7, v0, v1, v2, R5, 0x4A7484AA);
	P(v2, v3, v4, v5, v6, v7, v0, v1, R6, 0x5CB0A9DC);
	P(v1, v2, v3, v4, v5, v6, v7, v0, R7, 0x76F988DA);
	P(v0, v1, v2, v3, v4, v5, v6, v7, R8, 0x983E5152);
	P(v7, v0, v1, v2, v3, v4, v5, v6, R9, 0xA831C66D);
	P(v6, v7, v0, v1, v2, v3, v4, v5, R10, 0xB00327C8);
	P(v5, v6, v7, v0, v1, v2, v3, v4, R11, 0xBF597FC7);
	P(v4, v5, v6, v7, v0, v1, v2, v3, R12, 0xC6E00BF3);
	P(v3, v4, v5, v6, v7, v0, v1, v2, R13, 0xD5A79147);
	P(v2, v3, v4, v5, v6, v7, v0, v1, R14, 0x06CA6351);
	P(v1, v2, v3, v4, v5, v6, v7, v0, R15, 0x14292967);

	P(v0, v1, v2, v3, v4, v5, v6, v7, R0, 0x27B70A85);
	P(v7, v0, v1, v2, v3, v4, v5, v6, R1, 0x2E1B2138);
	P(v6, v7, v0, v1, v2, v3, v4, v5, R2, 0x4D2C6DFC);
	P(v5, v6, v7, v0, v1, v2, v3, v4, R3, 0x53380D13);
	P(v4, v5, v6, v7, v0, v1, v2, v3, R4, 0x650A7354);
	P(v3, v4, v5, v6, v7, v0, v1, v2, R5, 0x766A0ABB);
	P(v2, v3, v4, v5, v6, v7, v0, v1, R6, 0x81C2C92E);
	P(v1, v2, v3, v4, v5, v6, v7, v0, R7, 0x92722C85);
	P(v0, v1, v2, v3, v4, v5, v6, v7, R8, 0xA2BFE8A1);
	P(v7, v0, v1, v2, v3, v4, v5, v6, R9, 0xA81A664B);
	P(v6, v7, v0, v1, v2, v3, v4, v5, R10, 0xC24B8B70);
	P(v5, v6, v7, v0, v1, v2, v3, v4, R11, 0xC76C51A3);
	P(v4, v5, v6, v7, v0, v1, v2, v3, R12, 0xD192E819);
	P(v3, v4, v5, v6, v7, v0, v1, v2, R13, 0xD6990624);
	P(v2, v3, v4, v5, v6, v7, v0, v1, R14, 0xF40E3585);
	P(v1, v2, v3, v4, v5, v6, v7, v0, R15, 0x106AA070);

	P(v0, v1, v2, v3, v4, v5, v6, v7, R0, 0x19A4C116);
	P(v7, v0, v1, v2, v3, v4, v5, v6, R1, 0x1E376C08);
	P(v6, v7, v0, v1, v2, v3, v4, v5, R2, 0x2748774C);
	P(v5, v6, v7, v0, v1, v2, v3, v4, R3, 0x34B0BCB5);
	P(v4, v5, v6, v7, v0, v1, v2, v3, R4, 0x391C0CB3);
	P(v3, v4, v5, v6, v7, v0, v1, v2, R5, 0x4ED8AA4A);
	P(v2, v3, v4, v5, v6, v7, v0, v1, R6, 0x5B9CCA4F);
	P(v1, v2, v3, v4, v5, v6, v7, v0, R7, 0x682E6FF3);
	P(v0, v1, v2, v3, v4, v5, v6, v7, R8, 0x748F82EE);
	P(v7, v0, v1, v2, v3, v4, v5, v6, R9, 0x78A5636F);
	P(v6, v7, v0, v1, v2, v3, v4, v5, R10, 0x84C87814);
	P(v5, v6, v7, v0, v1, v2, v3, v4, R11, 0x8CC70208);
	P(v4, v5, v6, v7, v0, v1, v2, v3, R12, 0x90BEFFFA);
	P(v3, v4, v5, v6, v7, v0, v1, v2, R13, 0xA4506CEB);
	P(v2, v3, v4, v5, v6, v7, v0, v1, RD14, 0xBEF9A3F7);
	P(v1, v2, v3, v4, v5, v6, v7, v0, RD15, 0xC67178F2);
#undef v0
#undef v1
#undef v2
#undef v3
#undef v4
#undef v5
#undef v6
#undef v7
	return (res + state);
}



static __constant  uint8 H256 = {
	0x6A09E667, 0xBB67AE85, 0x3C6EF372,
	0xA54FF53A, 0x510E527F, 0x9B05688C,
	0x1F83D9AB, 0x5BE0CD19
};


static __constant uint16 pad_data =
{
	0x00000000, 0x00000000, 0x80000000, 0x00000000,
	0x00000000, 0x00000000, 0x00000000, 0x00000000,
	0x00000000, 0x00000000, 0x00000000, 0x00000000,
	0x00000000, 0x00000000, 0x00000000, 0x00000640
};

static __constant uint8 pad_state =
{
	0x80000000, 0x00000000, 0x00000000, 0x00000000,
	0x00000000, 0x00000000, 0x00000000, 0x00000100
};




__attribute__((reqd_work_group_size(WORKSIZE, 1, 1)))
__kernel void search(
    __global volatile uint* restrict ethash_output,
    __constant uint2 const* ethash_header,
    __global ulong8 const* ethash_dag,
    uint ethash_dag_size,
    ulong ethash_start_nonce,
    ulong ethash_target,
    volatile uint ethash_isolate,

	__constant const uchar* restrict pascal_input,
	__global volatile uint* restrict pascal_output, 
	const uint pascal_start_nonce, 
	const ulong pascal_target, 
	__constant uint8* pascal_midstate,
	const uint pascal_ratio
    )
{
    __global hash128_t const* g_dag = (__global hash128_t const*) ethash_dag;
	const uint gid = get_global_id(0) / 256 * 192 + (get_local_id(0));
    const uint thread_id = get_local_id(0) % 4;
    const uint hash_id = get_local_id(0) / 4;

    __local compute_hash_share sharebuf[WORKSIZE / 4];
#ifdef LEGACY
    __local uint buffer[WORKSIZE / 4];
#else
    __local uint buffer[WORKSIZE];
#endif
    __local compute_hash_share * const share = sharebuf + hash_id;

    // sha3_512(header .. nonce)
    uint2 state[25];

	if (get_local_id(0) < 192) {
		for (uint i = 0; i < 4; i++)
			state[i] = ethash_header[i];
		state[4] = as_uint2(ethash_start_nonce + gid);
		state[5] = as_uint2(0x0000000000000001UL);
		state[6] = (uint2)(0);
		state[7] = (uint2)(0);
		state[8] = as_uint2(0x8000000000000000UL);
		for (uint i = 9; i < 25; i++)
			state[i] = (uint2)(0);

		KECCAK_PROCESS(state, 5, 8, ethash_isolate);
	}
    
    uint init0;
    uint8 mix;
    
    #pragma unroll 1
    for (uint tid = 0; tid < 4; tid++) {
        // share init with other threads

		if (get_local_id(0) < 192)
			if (tid == thread_id)
            for (uint i = 0; i < 8; i++)
                share->uint2s[i] = state[i];
    
        barrier(CLK_LOCAL_MEM_FENCE);
        

		if (get_local_id(0) < 192) {
			// It's done like it was because of the duplication
			// We can do the same - with uint8s.
			mix = share->uint8s[thread_id & 1];

			init0 = share->uints[0];
		}

        barrier(CLK_LOCAL_MEM_FENCE);

        if (get_local_id(0) < 192) {
#ifdef LEGACY
			for (uint a = 0; a < (ACCESSES & ethash_isolate); a += 8) {
#else
#pragma unroll 1
			for (uint a = 0; a < ACCESSES; a += 8) {
#endif
				const uint lane_idx = 4 * hash_id + a / 8 % 4;
				MIX(0);
				MIX(1);
				MIX(2);
				MIX(3);
				MIX(4);
				MIX(5);
				MIX(6);
				MIX(7);
			}
		}
		else
		{
#pragma unroll 1
			for (volatile uint iter = 0; iter < pascal_ratio; ++iter) {
				uint nonce = ((get_global_id(0) / 256 * 64 + (get_local_id(0) - 192)) * 4 + tid) * pascal_ratio + iter + pascal_start_nonce;
				uint16 in;
				uint8 state1;
#pragma unroll 1
				for (volatile uint pass = 0; pass < 2; ++pass) {
					if (pass) {
						in.lo = state1;
						in.hi = pad_state;
					}
					else {
						in = pad_data;
						in.s0 = SWAP32(((__constant const uint *)pascal_input)[48]);
						in.s1 = SWAP32(nonce);
					}
					state1 = sha256_Transform(in, pass ? H256 : *pascal_midstate);
				}

				if (as_ulong(state1.s10) <= pascal_target) {
					pascal_output[atomic_inc(pascal_output + 0xFF)] = nonce;
				}
			}
		}

        barrier(CLK_LOCAL_MEM_FENCE);
        
		if (get_local_id(0) < 192)
			share->uint2s[thread_id] = (uint2)(fnv_reduce(mix.lo), fnv_reduce(mix.hi));
         
        barrier(CLK_LOCAL_MEM_FENCE);

		if (get_local_id(0) < 192)
        if (tid == thread_id)
            for (uint i = 0; i < 4; i++)
                state[8 + i] = share->uint2s[i];

        barrier(CLK_LOCAL_MEM_FENCE);
    }
    
	if (get_local_id(0) < 192) {
		for (uint i = 13; i < 25; ++i)
			state[i] = (uint2)(0);
		state[12] = as_uint2(0x0000000000000001UL);
		state[16] = as_uint2(0x8000000000000000UL);

		KECCAK_PROCESS(state, 12, 1, ethash_isolate);

		if (as_ulong(as_uchar8(state[0]).s76543210) < ethash_target) {
			uint slot = min(MAX_OUTPUTS - 1u, atomic_inc(&ethash_output[MAX_OUTPUTS]));
			ethash_output[slot] = gid;
		}
	}
}




typedef union _Node
{
    uint dwords[16];
    uint2 qwords[8];
    uint4 dqwords[4];
} Node;

static void SHA3_512(uint2 *s, uint ethash_isolate)
{
    uint2 st[25];
    
    for (uint i = 0; i < 8; ++i)
        st[i] = s[i];
    
    for (uint i = 8; i != 25; ++i)
        st[i] = (uint2)(0);
    
    st[8].x = 0x00000001;
    st[8].y = 0x80000000;
    KECCAK_PROCESS(st, 8, 8, ethash_isolate);
    
    for (uint i = 0; i < 8; ++i)
        s[i] = st[i];
}

__kernel void GenerateDAG(uint start, __global const uint16 *_Cache, __global uint16 *_DAG, uint LIGHT_SIZE, /* uint ethash_dag_size, */ uint ethash_isolate)
{
    __global const Node *Cache = (__global const Node *) _Cache;
    __global Node *DAG = (__global Node *) _DAG;
    uint NodeIdx = start + get_global_id(0);

	Node DAGNode = Cache[NodeIdx % LIGHT_SIZE];

	DAGNode.dwords[0] ^= NodeIdx;
	SHA3_512(DAGNode.qwords, ethash_isolate);

	for (uint i = 0; i < 256; ++i)
	{
		uint ParentIdx = fnv(NodeIdx ^ i, DAGNode.dwords[i & 15]) % LIGHT_SIZE;
		__global const Node *ParentNode = Cache + ParentIdx;

#pragma unroll
		for (uint x = 0; x < 4; ++x)
		{
			DAGNode.dqwords[x] *= (uint4)(FNV_PRIME);
			DAGNode.dqwords[x] ^= ParentNode->dqwords[x];
		}
	}

	SHA3_512(DAGNode.qwords, ethash_isolate);

	//if (NodeIdx < ethash_dag_size)
		DAG[NodeIdx] = DAGNode;
}
