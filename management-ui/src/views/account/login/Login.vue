<template>
  <div class="main">
    <a-form class="user-layout-login">
      <a-form-item>
        <a-input size="large" type="text" placeholder="账户" v-model="login_form.username">
          <a-icon slot="prefix" type="user" :style="{ color: 'rgba(0,0,0,.25)' }" />
        </a-input>
      </a-form-item>

      <a-form-item>
        <a-input
          size="large"
          type="password"
          autocomplete="false"
          placeholder="密码"
          v-model="login_form.password"
        >
          <a-icon slot="prefix" type="lock" :style="{ color: 'rgba(0,0,0,.25)' }" />
        </a-input>
      </a-form-item>

      <a-form-item v-if="false">
        <a-checkbox>自动登录</a-checkbox>
        <router-link
          :to="{ name: 'recover', params: { user: 'aaa'} }"
          class="forge-password"
          style="float: right;margin-left:10px"
        >忘记密码</router-link>
        <router-link class="register" style="float: right;" :to="{ name: 'register' }">注册账户</router-link>
      </a-form-item>

      <a-form-item style="margin-top:24px">
        <a-button
          size="large"
          type="primary"
          class="login-button"
          @click="()=>{login()}"
          :loading="login_form.loading"
        >确定</a-button>
      </a-form-item>
    </a-form>
  </div>
</template>

<script>
import { post_form } from '@/api/ajax'

export default {
  data() {
    return {
      login_form: {
        loading: false,
        username: '',
        password: ''
      }
    }
  },
  methods: {
    login() {
      this.login_form.loading = true
      post_form(this, '/api/member/admin/Account/LoginViaPass', this.login_form)
        .then(res => {
          if (res.Success) {
            this.$store.commit('GET_INFO', this)
            this.$router.push({ name: 'dashboard' })
            // 延迟 1 秒显示欢迎信息
            setTimeout(() => {
              this.$notification.success({
                message: '欢迎',
                description: `欢迎回来`
              })
            }, 1000)
          } else {
            this.$notification['error']({
              message: '错误',
              description: res.ErrorMsg,
              duration: 4
            })
          }
        })
        .finally(() => {
          this.login_form.loading = false
        })
    }
  }
}
</script>

<style lang="less" scoped>
.user-layout-login {
  label {
    font-size: 14px;
  }

  .forge-password {
    font-size: 14px;
  }

  button.login-button {
    padding: 0 15px;
    font-size: 16px;
    height: 40px;
    width: 100%;
  }

  .user-login-other {
    text-align: left;
    margin-top: 24px;
    line-height: 22px;

    .item-icon {
      font-size: 24px;
      color: rgba(0, 0, 0, 0.2);
      margin-left: 16px;
      vertical-align: middle;
      cursor: pointer;
      transition: color 0.3s;

      &:hover {
        color: #1890ff;
      }
    }

    .register {
      float: right;
    }
  }
}
</style>
