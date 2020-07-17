<template>
  <div>
    <a-form layout="vertical">
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="Name">
            <a-input placeholder="Please enter user name" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="Url">
            <a-input
              style="width: 100%"
              addonBefore="http://"
              addonAfter=".com"
              placeholder="please enter url"
            />
          </a-form-item>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="假期类型">
            <a-select placeholder="Please a-s an owner">
              <a-select-option value="xiao">年假</a-select-option>
              <a-select-option value="mao">事假</a-select-option>
              <a-select-option value="bing">病假</a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="Type">
            <a-select placeholder="Please choose the type">
              <a-select-option value="private">Private</a-select-option>
              <a-select-option value="public">Public</a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="Approver">
            <a-select placeholder="Please choose the approver">
              <a-select-option value="jack">Jack Ma</a-select-option>
              <a-select-option value="tom">Tom Liu</a-select-option>
            </a-select>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="DateTime">
            <a-date-picker style="width: 100%" :getPopupContainer="trigger => trigger.parentNode" />
          </a-form-item>
        </a-col>
      </a-row>
      <a-row :gutter="16">
        <a-col :span="24">
          <a-form-item label="Description">
            <a-textarea :rows="4" placeholder="please enter url description" />
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
  </div>
</template>

<script>
export default {}
</script>