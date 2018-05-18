


#define SWAP32(x)		as_uint(as_uchar4(x).s3210)
#define SWAP64(x)		as_ulong(as_uchar8(x).s76543210)

#if defined(cl_nv_pragma_unroll)
uint amd_bitalign(uint src0, uint src1, uint src2)
{
	uint dest;
	asm("shf.r.wrap.b32 %0, %2, %1, %3;" : "=r"(dest) : "r"(src0), "r"(src1), "r"(src2));
	return dest;
}
uint2 amd_bitalign_uint2(uint2 src0, uint2 src1, uint src2)
{
	return (uint2) {
		amd_bitalign(src0.s0, src1.s0, src2),
			amd_bitalign(src0.s1, src1.s1, src2)
	};
}
#elif !defined(cl_amd_media_ops)
uint amd_bitalign(uint src0, uint src1, uint src2)
{
	return (uint) (((((long)src0) << 32) | (long)src1) >> (src2 & 31));
}
uint2 amd_bitalign_uint2(uint2 src0, uint2 src1, uint src2)
{
	return (uint2) {
		(uint) (((((long)src0.s0) << 32) | (long)src1.s0) >> (src2 & 31)),
			(uint) (((((long)src0.s1) << 32) | (long)src1.s1) >> (src2 & 31))
	};
}
#else
#define amd_bitalign_uint2 amd_bitalign
#endif



#include "sha256.cl"
#include "ripemd160.cl"



static const __constant ulong K512[80] =
{
	0x428A2F98D728AE22UL, 0x7137449123EF65CDUL,
	0xB5C0FBCFEC4D3B2FUL, 0xE9B5DBA58189DBBCUL,
	0x3956C25BF348B538UL, 0x59F111F1B605D019UL,
	0x923F82A4AF194F9BUL, 0xAB1C5ED5DA6D8118UL,
	0xD807AA98A3030242UL, 0x12835B0145706FBEUL,
	0x243185BE4EE4B28CUL, 0x550C7DC3D5FFB4E2UL,
	0x72BE5D74F27B896FUL, 0x80DEB1FE3B1696B1UL,
	0x9BDC06A725C71235UL, 0xC19BF174CF692694UL,
	0xE49B69C19EF14AD2UL, 0xEFBE4786384F25E3UL,
	0x0FC19DC68B8CD5B5UL, 0x240CA1CC77AC9C65UL,
	0x2DE92C6F592B0275UL, 0x4A7484AA6EA6E483UL,
	0x5CB0A9DCBD41FBD4UL, 0x76F988DA831153B5UL,
	0x983E5152EE66DFABUL, 0xA831C66D2DB43210UL,
	0xB00327C898FB213FUL, 0xBF597FC7BEEF0EE4UL,
	0xC6E00BF33DA88FC2UL, 0xD5A79147930AA725UL,
	0x06CA6351E003826FUL, 0x142929670A0E6E70UL,
	0x27B70A8546D22FFCUL, 0x2E1B21385C26C926UL,
	0x4D2C6DFC5AC42AEDUL, 0x53380D139D95B3DFUL,
	0x650A73548BAF63DEUL, 0x766A0ABB3C77B2A8UL,
	0x81C2C92E47EDAEE6UL, 0x92722C851482353BUL,
	0xA2BFE8A14CF10364UL, 0xA81A664BBC423001UL,
	0xC24B8B70D0F89791UL, 0xC76C51A30654BE30UL,
	0xD192E819D6EF5218UL, 0xD69906245565A910UL,
	0xF40E35855771202AUL, 0x106AA07032BBD1B8UL,
	0x19A4C116B8D2D0C8UL, 0x1E376C085141AB53UL,
	0x2748774CDF8EEB99UL, 0x34B0BCB5E19B48A8UL,
	0x391C0CB3C5C95A63UL, 0x4ED8AA4AE3418ACBUL,
	0x5B9CCA4F7763E373UL, 0x682E6FF3D6B2B8A3UL,
	0x748F82EE5DEFB2FCUL, 0x78A5636F43172F60UL,
	0x84C87814A1F0AB72UL, 0x8CC702081A6439ECUL,
	0x90BEFFFA23631E28UL, 0xA4506CEBDE82BDE9UL,
	0xBEF9A3F7B2C67915UL, 0xC67178F2E372532BUL,
	0xCA273ECEEA26619CUL, 0xD186B8C721C0C207UL,
	0xEADA7DD6CDE0EB1EUL, 0xF57D4F7FEE6ED178UL,
	0x06F067AA72176FBAUL, 0x0A637DC5A2C898A6UL,
	0x113F9804BEF90DAEUL, 0x1B710B35131C471BUL,
	0x28DB77F523047D84UL, 0x32CAAB7B40C72493UL,
	0x3C9EBE0A15C9BEBCUL, 0x431D67C49C100D4CUL,
	0x4CC5D4BECB3E42B6UL, 0x597F299CFC657E2AUL,
	0x5FCB6FAB3AD6FAECUL, 0x6C44198C4A475817UL
};

static const __constant ulong SHA512_INIT[8] =
{
	0x6A09E667F3BCC908UL, 0xBB67AE8584CAA73BUL,
	0x3C6EF372FE94F82BUL, 0xA54FF53A5F1D36F1UL,
	0x510E527FADE682D1UL, 0x9B05688C2B3E6C1FUL,
	0x1F83D9ABFB41BD6BUL, 0x5BE0CD19137E2179UL
};

#define ROTR64(x, y)	rotate((x), 64UL - (y))

ulong FAST_ROTR64_LO(const uint2 x, const uint y) { return(as_ulong(amd_bitalign_uint2(x.s10, x, y))); }
ulong FAST_ROTR64_HI(const uint2 x, const uint y) { return(as_ulong(amd_bitalign_uint2(x, x.s10, (y - 32)))); }


#define BSG5_0(x) (FAST_ROTR64_LO(as_uint2(x), 28) ^ FAST_ROTR64_HI(as_uint2(x), 34) ^ FAST_ROTR64_HI(as_uint2(x), 39))
#define BSG5_1(x) (FAST_ROTR64_LO(as_uint2(x), 14) ^ FAST_ROTR64_LO(as_uint2(x), 18) ^ FAST_ROTR64_HI(as_uint2(x), 41))
#define SSG5_0(x) (FAST_ROTR64_LO(as_uint2(x), 1) ^ FAST_ROTR64_LO(as_uint2(x), 8) ^ ((x) >> 7))
#define SSG5_1(x) (FAST_ROTR64_LO(as_uint2(x), 19) ^ FAST_ROTR64_HI(as_uint2(x), 61) ^ ((x) >> 6))

#define CH(X, Y, Z) bitselect(Z, Y, X)
#define MAJ(X, Y, Z) CH((X ^ Z), Y, Z)

void SHA2_512_STEP2(const ulong *W, uint ord, ulong *r, int i)
{
	ulong T1;
	int x = 8 - ord;

	ulong a = r[x & 7], b = r[(x + 1) & 7], c = r[(x + 2) & 7], d = r[(x + 3) & 7];
	ulong e = r[(x + 4) & 7], f = r[(x + 5) & 7], g = r[(x + 6) & 7], h = r[(x + 7) & 7];

	T1 = h + BSG5_1(e) + CH(e, f, g) + W[i] + K512[i];
	r[(3 + x) & 7] = d + T1;
	r[(7 + x) & 7] = T1 + BSG5_0(a) + MAJ(a, b, c);
}

void SHA512Block(ulong *W, ulong *buf)
{
	ulong r[8];

	for (int i = 0; i < 8; ++i) r[i] = buf[i];

	//for (int i = 0; i < 16; ++i) W[i] = data[i];

#pragma unroll 4
	for (int i = 16; i < 80; ++i) W[i] = SSG5_1(W[i - 2]) + W[i - 7] + SSG5_0(W[i - 15]) + W[i - 16];

#pragma unroll 1
	for (int i = 0; i < 80; i += 8)
	{
#pragma unroll
		for (int j = 0; j < 8; ++j)
		{
			SHA2_512_STEP2(W, j, r, i + j);
		}
	}

	for (int i = 0; i < 8; ++i) buf[i] += r[i];
}



__attribute__((reqd_work_group_size(WORKSIZE, 1, 1)))
__kernel void search_combined(__global const uint *lbry_input, __global uint *lbry_output, uint lbry_start_nonce, ulong lbry_target)
{
	uint SHA256Buf[16];
	ulong W[80], SHA512Out[8];
	uint RMD160_0[16];
	uint RMD160_0_Out[5];
	uint8 outbuf;
		
	for (int iter = 0; iter < ITERATIONS; ++iter) {

		uint gid = get_global_id(0) * ITERATIONS + iter + lbry_start_nonce;


#pragma unroll
		for (int i = 0; i < 16; ++i) SHA256Buf[i] = lbry_input[i];
		outbuf = (uint8)(0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A, 0x510E527F, 0x9B05688C, 0x1F83D9AB, 0x5BE0CD19);

#pragma unroll 1
		for (volatile int i = 0; i < 3; ++i)
		{
			if (i == 1)
			{
				SHA256Buf[0] = lbry_input[0 + 16];
				SHA256Buf[1] = lbry_input[1 + 16];
				SHA256Buf[2] = lbry_input[2 + 16];
				SHA256Buf[3] = lbry_input[3 + 16];
				SHA256Buf[4] = lbry_input[4 + 16];
				SHA256Buf[5] = lbry_input[5 + 16];
				SHA256Buf[6] = lbry_input[6 + 16];
				SHA256Buf[7] = lbry_input[7 + 16];
				SHA256Buf[8] = lbry_input[8 + 16];
				SHA256Buf[9] = lbry_input[9 + 16];
				SHA256Buf[10] = lbry_input[10 + 16];
				SHA256Buf[11] = SWAP32(gid);
				SHA256Buf[12] = 0x80000000;
				SHA256Buf[13] = 0x00000000;
				SHA256Buf[14] = 0x00000000;
				SHA256Buf[15] = 0x00000380;
			}
			if (i == 2)
			{
				SHA256Buf[0] = outbuf.s0;
				SHA256Buf[1] = outbuf.s1;
				SHA256Buf[2] = outbuf.s2;
				SHA256Buf[3] = outbuf.s3;
				SHA256Buf[4] = outbuf.s4;
				SHA256Buf[5] = outbuf.s5;
				SHA256Buf[6] = outbuf.s6;
				SHA256Buf[7] = outbuf.s7;
				SHA256Buf[8] = 0x80000000;
				SHA256Buf[9] = 0x00000000;
				SHA256Buf[10] = 0x00000000;
				SHA256Buf[11] = 0x00000000;
				SHA256Buf[12] = 0x00000000;
				SHA256Buf[13] = 0x00000000;
				SHA256Buf[14] = 0x00000000;
				SHA256Buf[15] = 0x00000100;
				outbuf = (uint8)(0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A, 0x510E527F, 0x9B05688C, 0x1F83D9AB, 0x5BE0CD19);
			}
			outbuf = sha256_round(((uint16 *)SHA256Buf)[0], outbuf);
		}



		/////////////
		// SHA-512 //
		/////////////

		for (int i = 0; i < 16; ++i) W[i] = 0UL;
		((uint8 *)W)[0].s0 = outbuf.s1;
		((uint8 *)W)[0].s1 = outbuf.s0;
		((uint8 *)W)[0].s2 = outbuf.s3;
		((uint8 *)W)[0].s3 = outbuf.s2;
		((uint8 *)W)[0].s4 = outbuf.s5;
		((uint8 *)W)[0].s5 = outbuf.s4;
		((uint8 *)W)[0].s6 = outbuf.s7;
		((uint8 *)W)[0].s7 = outbuf.s6;
		W[4] = 0x8000000000000000UL;
		W[15] = 0x0000000000000100UL;
		for (int i = 0; i < 8; ++i) SHA512Out[i] = SHA512_INIT[i];
		SHA512Block(W, SHA512Out);
		for (int i = 0; i < 8; ++i) SHA512Out[i] = SWAP64(SHA512Out[i]);



		////////////
		// RMD160 //
		////////////

		for (int i = 0; i < 16; ++i)
			RMD160_0[i] = 0;
		for (int i = 0; i < 4; ++i)
			((ulong *)RMD160_0)[i] = SHA512Out[i];
		RMD160_0[8] = 0x00000080;
		RMD160_0[14] = 0x00000100;
		for (int i = 0; i < 5; ++i)
			RMD160_0_Out[i] = RMD160_IV[i];

#pragma unroll 1
		for (volatile int j = 0; j < 2; ++j)
		{
			if (j == 1)
			{
				SHA256Buf[0] = SWAP32(RMD160_0_Out[0]);
				SHA256Buf[1] = SWAP32(RMD160_0_Out[1]);
				SHA256Buf[2] = SWAP32(RMD160_0_Out[2]);
				SHA256Buf[3] = SWAP32(RMD160_0_Out[3]);
				SHA256Buf[4] = SWAP32(RMD160_0_Out[4]);

				for (int i = 0; i < 16; ++i)
					RMD160_0[i] = 0;
				for (int i = 0; i < 4; ++i)
					((ulong *)RMD160_0)[i] = SHA512Out[i + 4];
				RMD160_0[8] = 0x00000080;
				RMD160_0[14] = 0x00000100;
				for (int i = 0; i < 5; ++i)
					RMD160_0_Out[i] = RMD160_IV[i];
			}
			RIPEMD160_ROUND_BODY(RMD160_0, RMD160_0_Out);
		}



		/////////////
		// SHA-256 //
		/////////////

		SHA256Buf[5] = SWAP32(RMD160_0_Out[0]);
		SHA256Buf[6] = SWAP32(RMD160_0_Out[1]);
		SHA256Buf[7] = SWAP32(RMD160_0_Out[2]);
		SHA256Buf[8] = SWAP32(RMD160_0_Out[3]);
		SHA256Buf[9] = SWAP32(RMD160_0_Out[4]);
		SHA256Buf[10] = 0x80000000;
		for (int i = 11; i < 15; ++i) SHA256Buf[i] = 0x00000000U;
		SHA256Buf[15] = 0x00000140;
		outbuf = (uint8)(0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A, 0x510E527F, 0x9B05688C, 0x1F83D9AB, 0x5BE0CD19);
#pragma unroll 1
		for (volatile int j = 0; j < 2; ++j)
		{
			if (j == 1)
			{
				((uint8 *)SHA256Buf)[0] = outbuf;
				SHA256Buf[8] = 0x80000000;
				for (int i = 9; i < 15; ++i) SHA256Buf[i] = 0x00000000;
				SHA256Buf[15] = 0x00000100;

				outbuf = (uint8)(0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A, 0x510E527F, 0x9B05688C, 0x1F83D9AB, 0x5BE0CD19);
			}
			outbuf = sha256_round(((uint16 *)SHA256Buf)[0], outbuf);
		}



		////////////
		// OUTPUT //
		////////////

		outbuf.s6 = SWAP32(outbuf.s6);
		outbuf.s7 = SWAP32(outbuf.s7);

		if ((((ulong)outbuf.s7 << 32) | outbuf.s6) <= lbry_target) {
			int i = atomic_inc(lbry_output + 0xFF);
			lbry_output[i] = gid;
			lbry_output[256 + i * 8 + 0] = SWAP32(outbuf.s0);
			lbry_output[256 + i * 8 + 1] = SWAP32(outbuf.s1);
			lbry_output[256 + i * 8 + 2] = SWAP32(outbuf.s2);
			lbry_output[256 + i * 8 + 3] = SWAP32(outbuf.s3);
			lbry_output[256 + i * 8 + 4] = SWAP32(outbuf.s4);
			lbry_output[256 + i * 8 + 5] = SWAP32(outbuf.s5);
			lbry_output[256 + i * 8 + 6] = outbuf.s6;
			lbry_output[256 + i * 8 + 7] = outbuf.s7;
		}
	}
}
