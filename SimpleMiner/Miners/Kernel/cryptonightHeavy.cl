#define CRYPTONIGHT_HEAVY

#define  mix_and_propagate(xin) (xin)[(get_local_id(1)) % 8][get_local_id(0)] ^ (xin)[(get_local_id(1) + 1) % 8][get_local_id(0)]

#define MEMORY (4UL * 1024 * 1024)
#define MASK 0x3FFFF0
#define ITERATIONS 0x40000

#include "cryptonight.cl"
