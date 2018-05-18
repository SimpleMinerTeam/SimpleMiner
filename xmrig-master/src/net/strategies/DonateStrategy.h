/* XMRig
 * Copyright 2010      Jeff Garzik <jgarzik@pobox.com>
 * Copyright 2012-2014 pooler      <pooler@litecoinpool.org>
 * Copyright 2014      Lucas Jones <https://github.com/lucasjones>
 * Copyright 2014-2016 Wolf9466    <https://github.com/OhGodAPet>
 * Copyright 2016      Jay D Dee   <jayddee246@gmail.com>
 * Copyright 2017-2018 XMR-Stak    <https://github.com/fireice-uk>, <https://github.com/psychocrypt>
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

#ifndef __DONATESTRATEGY_H__
#define __DONATESTRATEGY_H__


#include <uv.h>
#include <vector>


#include "common/net/Pool.h"
#include "interfaces/IClientListener.h"
#include "interfaces/IStrategy.h"
#include "interfaces/IStrategyListener.h"


class Client;
class IStrategyListener;
class Url;


class DonateStrategy : public IStrategy, public IStrategyListener
{
public:
    DonateStrategy(int level, const char *user, xmrig::Algo algo, IStrategyListener *listener);
    ~DonateStrategy();

public:
    inline bool isActive() const override  { return m_active; }
    inline void resume() override          {}

    int64_t submit(const JobResult &result) override;
    void connect() override;
    void stop() override;
    void tick(uint64_t now) override;

protected:
    void onActive(IStrategy *strategy, Client *client) override;
    void onJob(IStrategy *strategy, Client *client, const Job &job) override;
    void onPause(IStrategy *strategy) override;
    void onResultAccepted(IStrategy *strategy, Client *client, const SubmitResult &result, const char *error) override;

private:
    void idle(uint64_t timeout);
    void suspend();

    static void onTimer(uv_timer_t *handle);

    bool m_active;
    const int m_donateTime;
    const int m_idleTime;
    IStrategy *m_strategy;
    IStrategyListener *m_listener;
    std::vector<Pool> m_pools;
    uv_timer_t m_timer;
};

#endif /* __DONATESTRATEGY_H__ */
