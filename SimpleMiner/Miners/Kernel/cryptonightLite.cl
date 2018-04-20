#define CRYPTONIGHT_LIGHT

#define mix_and_propagate(xin) (xin)[(get_local_id(1)) % 8][get_local_id(0)] ^ (xin)[(get_local_id(1) + 1) % 8][get_local_id(0)]

#define MEMORY (1UL * 1024 * 1024)
#define MASK 0xFFFF0
#define ITERATIONS 0x40000

#include "cryptonight.cl"
