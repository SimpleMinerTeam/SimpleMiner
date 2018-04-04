/* XMRig
 * Copyright 2010      Jeff Garzik <jgarzik@pobox.com>
 * Copyright 2012-2014 pooler      <pooler@litecoinpool.org>
 * Copyright 2014      Lucas Jones <https://github.com/lucasjones>
 * Copyright 2014-2016 Wolf9466    <https://github.com/OhGodAPet>
 * Copyright 2016      Jay D Dee   <jayddee246@gmail.com>
 * Copyright 2017-2018 XMR-Stak    <https://github.com/fireice-uk>, <https://github.com/psychocrypt>
 * Copyright 2018      Lee Clagett <https://github.com/vtnerd>
 * Copyright 2016-2018 XMRig       <https://github.com/xmrig>, <support@xmrig.com>
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

#ifndef __CRYPTONIGHT_H__
#define __CRYPTONIGHT_H__


#include <stddef.h>
#include <stdint.h>


#define AEON_MEMORY   1048576
#define AEON_MASK     0xFFFF0
#define AEON_ITER     0x40000

#define MONERO_MEMORY 2097152
#define MONERO_MASK   0x1FFFF0
#define MONERO_ITER   0x80000


struct cryptonight_ctx {
    alignas(16) uint8_t state0[200];
    alignas(16) uint8_t state1[200];
    alignas(16) uint8_t* memory;
};


class Job;
class JobResult;


class CryptoNight
{
public:
    static bool hash(const Job &job, JobResult &result, cryptonight_ctx *ctx);
    static bool init(int algo, int variant);
    static void hash(const uint8_t *input, size_t size, uint8_t *output, cryptonight_ctx *ctx, int variant);

private:
    static bool selfTest(int algo);
};

#endif /* __CRYPTONIGHT_H__ */
